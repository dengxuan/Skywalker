using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Skywalker.Ddd.Application;
using System.Collections.Immutable;

namespace Skywalker.CodeAnalyzers.Analyzers;

internal sealed class CompilationAnalyzer
{
    private static readonly SymbolEqualityComparer _symbolComparer = SymbolEqualityComparer.Default;
    private readonly GeneratorExecutionContext _context;
    private readonly Compilation _compilation;
    private readonly HashSet<RequestMessage> _requestMessages;
    private readonly HashSet<CommandMessage> _notificationMessages;
    private readonly HashSet<RequestMessageHandler> _requestMessageHandlers;
    private readonly HashSet<CommandMessageHandler> _notificationMessageHandlers;

    public readonly ImmutableArray<RequestMessageHandlerWrapper> RequestMessageHandlerWrappers;

    private readonly INamedTypeSymbol[] _baseHandlerSymbols;
    private readonly INamedTypeSymbol[] _baseMessageSymbols;

    private readonly INamedTypeSymbol _notificationHandlerInterfaceSymbol;
    private readonly INamedTypeSymbol _notificationInterfaceSymbol;

    public IEnumerable<RequestMessage> RequestMessages =>
        _requestMessages.Where(r => r.Handler is not null);

    public IEnumerable<CommandMessage> NotificationMessages =>
        _notificationMessages;

    public IEnumerable<RequestMessageHandler> RequestMessageHandlers =>
        _requestMessageHandlers;

    public IEnumerable<CommandMessageHandler> NotificationMessageHandlers =>
        _notificationMessageHandlers.Where(h => !h.IsOpenGeneric);

    public IEnumerable<CommandMessageHandler> OpenGenericNotificationMessageHandlers =>
        _notificationMessageHandlers.Where(h => h.IsOpenGeneric);

    public bool HasRequests => _requestMessages.Any(r => r.Handler is not null && r.MessageType == "Request");
    public bool HasCommands => _requestMessages.Any(r => r.Handler is not null && r.MessageType == "Command");
    public bool HasQueries => _requestMessages.Any(r => r.Handler is not null && r.MessageType == "Query");

    public bool HasAnyRequest => HasRequests || HasCommands || HasQueries;

    public bool HasNotifications => _notificationMessages.Any();

    public IEnumerable<RequestMessage> IRequestMessages => _requestMessages.Where(r => r.Handler is not null && r.MessageType == "Request");
    public IEnumerable<RequestMessage> ICommandMessages => _requestMessages.Where(r => r.Handler is not null && r.MessageType == "Command");
    public IEnumerable<RequestMessage> IQueryMessages => _requestMessages.Where(r => r.Handler is not null && r.MessageType == "Query");

    public IEnumerable<RequestMessage> IMessages => _requestMessages.Where(r => r.Handler is not null);

    private bool _hasErrors;

    public bool HasErrors => _hasErrors;

    public INamedTypeSymbol UnitSymbol { get; }

    public Compilation Compilation => _compilation;

    public string ApplicationNamespace { get; private set; } = Constants.Namespace;

    public string GeneratorVersion { get; }

    public IFieldSymbol ServiceLifetimeSymbol { get; private set; }
    public IFieldSymbol SingletonServiceLifetimeSymbol { get; }

    public string ServiceLifetime => ServiceLifetimeSymbol.GetFieldSymbolFullName();

    public string SingletonServiceLifetime => SingletonServiceLifetimeSymbol.GetFieldSymbolFullName();

    public bool ServiceLifetimeIsSingleton => ServiceLifetimeSymbol.Name == "Singleton";

    public bool ServiceLifetimeIsScoped => ServiceLifetimeSymbol.Name == "Scoped";

    public CompilationAnalyzer(in GeneratorExecutionContext context, string generatorVersion)
    {
        _context = context;
        _compilation = context.Compilation;

        _requestMessages = new();
        _notificationMessages = new();
        _requestMessageHandlers = new();
        _notificationMessageHandlers = new();

        UnitSymbol = _compilation.GetTypeByMetadataName($"{Constants.Namespace}.Dtos.Unit")!.OriginalDefinition;

        _baseHandlerSymbols = new INamedTypeSymbol[]
        {
            // Handler
            _compilation.GetTypeByMetadataName($"{Constants.Namespace}.IQueryHandler`2")!.OriginalDefinition,
            _compilation.GetTypeByMetadataName($"{Constants.Namespace}.ICommandHandler`1")!.OriginalDefinition,
        };

        var serviceLifetimeSymbol = _compilation.GetTypeByMetadataName("Microsoft.Extensions.DependencyInjection.ServiceLifetime")!;
        SingletonServiceLifetimeSymbol = (IFieldSymbol)serviceLifetimeSymbol.GetMembers().Single(m => m.Name == "Singleton");
        ServiceLifetimeSymbol = SingletonServiceLifetimeSymbol;

        RequestMessageHandlerWrappers = new RequestMessageHandlerWrapper[]
        {
            new RequestMessageHandlerWrapper("Query", this),
        }.ToImmutableArray();

        _notificationHandlerInterfaceSymbol = _baseHandlerSymbols[_baseHandlerSymbols.Length - 1];

        _baseMessageSymbols = new INamedTypeSymbol[]
        {
            // Dto types
            _compilation.GetTypeByMetadataName($"{Constants.Namespace}.Dtos.IQuery`1")!.OriginalDefinition,
            _compilation.GetTypeByMetadataName($"{Constants.Namespace}.Dtos.ICommand")!.OriginalDefinition,
        };

        _notificationInterfaceSymbol = _baseMessageSymbols[_baseMessageSymbols.Length - 1];

        GeneratorVersion = generatorVersion;
    }

    public void Analyze(CancellationToken cancellationToken)
    {
        TryParseOptions(cancellationToken);

        var queue = new Queue<INamespaceOrTypeSymbol>();

        FindGlobalNamespaces(queue);

        PopulateMetadata(queue);
    }

    private void FindGlobalNamespaces(Queue<INamespaceOrTypeSymbol> queue)
    {
        queue.Enqueue(_compilation.Assembly.GlobalNamespace);

        foreach (var reference in _compilation.References)
        {
            if (_compilation.GetAssemblyOrModuleSymbol(reference) is not IAssemblySymbol assemblySymbol)
            {
                continue;
            }

            if (!assemblySymbol.Modules.Any(m => m.ReferencedAssemblies.Any(ra => ra.Name == Constants.Namespace)))
            {
                continue;
            }

            queue.Enqueue(assemblySymbol.GlobalNamespace);
        }
    }

    private void PopulateMetadata(Queue<INamespaceOrTypeSymbol> queue)
    {
        //System.Diagnostics.Debugger.Launch();
#pragma warning disable RS1024 // Compare symbols correctly
        var requestMessageHandlerMap = new Dictionary<INamedTypeSymbol, object?>(_symbolComparer);
#pragma warning restore RS1024 // Compare symbols correctly

        while (queue.Count > 0)
        {
            var nsOrTypeSymbol = queue.Dequeue();

            if (nsOrTypeSymbol is INamespaceSymbol nsSymbol)
            {
                foreach (var member in nsSymbol.GetMembers())
                {
                    ProcessMember(queue, member, requestMessageHandlerMap);
                }
            }
            else
            {
                ProcessMember(queue, (INamedTypeSymbol)nsOrTypeSymbol, requestMessageHandlerMap);
            }
        }

        foreach (var kvp in requestMessageHandlerMap)
        {
            if (kvp.Value is not RequestMessage message)
            {
                continue;
            }

            ReportDiagnostic(message.Symbol, (in GeneratorExecutionContext c, INamedTypeSymbol s) => c.ReportMessageWithoutHandler(s));
        }

        foreach (var notificationMessage in _notificationMessages)
        {
            foreach (var notificationMessageHandler in _notificationMessageHandlers)
            {
                if (notificationMessageHandler.IsOpenGeneric) // These are added as open generics
                {
                    continue;
                }

                foreach (var notificationMessageHandlerInterfaceSymbol in notificationMessageHandler.Symbol.AllInterfaces)
                {
                    if (notificationMessageHandlerInterfaceSymbol.ContainingNamespace.Name != Constants.Namespace)
                    {
                        continue;
                    }

                    if (!_symbolComparer.Equals(notificationMessageHandlerInterfaceSymbol.OriginalDefinition, _notificationHandlerInterfaceSymbol))
                    {
                        continue;
                    }

                    var candidateNotificationMessageSymbol = (INamedTypeSymbol)notificationMessageHandlerInterfaceSymbol.TypeArguments[0];

                    if (_symbolComparer.Equals(candidateNotificationMessageSymbol, notificationMessage.Symbol))
                    {
                        notificationMessage.AddHandlers(notificationMessageHandler);
                    }
                    else if (_compilation.HasImplicitConversion(notificationMessage.Symbol, candidateNotificationMessageSymbol))
                    {
                        notificationMessage.AddHandlers(notificationMessageHandler);
                    }
                }
            }

            // This diagnostic is not safe to use here.
            // A user can define a notification, expecting it to only
            // show up in an open generic handler.
            // We don't keep track of bindings between notification and
            // these open generic handlers, so we can't know what notifications
            // are and aren't handled just yet.
            // TODO - include open generic handlers in analysis as well, so that we can report this correctly.
            //if (notificationMessage.HandlerCount == 0)
            //{
            //    ReportDiagnostic(notificationMessage.Symbol, (in GeneratorExecutionContext c, INamedTypeSymbol s) => c.ReportMessageWithoutHandler(s));
            //}
        }

        const int NOT_RELEVANT = 0;
        const int IS_REQUEST_HANDLER = 1;
        const int IS_NOTIFICATION_HANDLER = 2;
        const int IS_REQUEST = 3;
        const int IS_NOTIFICATION = 4;

        void ProcessMember(Queue<INamespaceOrTypeSymbol> queue, INamespaceOrTypeSymbol member, Dictionary<INamedTypeSymbol, object?> mapping)
        {
            if (member is INamespaceSymbol childNsSymbol)
            {
                queue.Enqueue(childNsSymbol);
                return;
            }

            var typeSymbol = (INamedTypeSymbol)member;

            foreach (var childTypeSymbol in typeSymbol.GetTypeMembers())
            {
                queue.Enqueue(childTypeSymbol);
            }

            if (typeSymbol.IsStatic || typeSymbol.IsAbstract)
            {
                return;
            }

            if (typeSymbol.TypeKind is not TypeKind.Struct and not TypeKind.Class)
            {
                return;
            }

            for (int i = 0; i < typeSymbol.AllInterfaces.Length; i++)
            {
                var typeInterfaceSymbol = typeSymbol.AllInterfaces[i];
                var typeInterfaceNamespace = typeInterfaceSymbol.ContainingNamespace.ToDisplayString();
                if (typeInterfaceNamespace != Constants.Namespace && typeInterfaceNamespace != Constants.DtoNamespace)
                {
                    continue;
                }

                if (!ProcessInterface(i, typeSymbol, typeInterfaceSymbol, typeSymbol.TypeKind == TypeKind.Struct, mapping))
                {
                    break;
                }
            }
        }

        bool ProcessInterface(int i, INamedTypeSymbol typeSymbol, INamedTypeSymbol typeInterfaceSymbol, bool isStruct, Dictionary<INamedTypeSymbol, object?> mapping)
        {
            var codeOfInterest = IsInteresting(typeInterfaceSymbol);
            switch (codeOfInterest)
            {
                case NOT_RELEVANT: break; // Continue loop
                case IS_REQUEST_HANDLER:
                case IS_NOTIFICATION_HANDLER:
                    {
                        if (isStruct)
                        {
                            // Handlers must be classes
                            ReportDiagnostic(typeSymbol, (in GeneratorExecutionContext c, INamedTypeSymbol s) => c.ReportInvalidHandlerType(s));
                            return false;
                        }

                        // TODO, return here? AllInterfaces ordered?
                        if (IsAlreadyHandledByDerivedInterface(i, 1, typeSymbol, typeInterfaceSymbol))
                        {
                            break;
                        }

                        var isRequest = codeOfInterest == IS_REQUEST_HANDLER;

                        if (isRequest)
                        {
                            if (IsOpenGeneric(typeSymbol))
                            {
                                // Handlers must be classes
                                ReportDiagnostic(typeSymbol, (in GeneratorExecutionContext c, INamedTypeSymbol s) => c.ReportOpenGenericRequestHandler(s));
                                return false;
                            }

                            var messageType = typeInterfaceSymbol.Name.Substring(1, typeInterfaceSymbol.Name.IndexOf("Handler") - 1);

                            var handler = new RequestMessageHandler(typeSymbol, messageType, this);
                            var requestMessageSymbol = (INamedTypeSymbol)typeInterfaceSymbol.TypeArguments[0];
                            if (mapping.TryGetValue(requestMessageSymbol, out var requestMessageObj))
                            {
                                if (requestMessageObj is null || requestMessageObj is not RequestMessage requestMessage)
                                {
                                    // Signal that we have duplicates
                                    ReportDiagnostic(typeSymbol, (in GeneratorExecutionContext c, INamedTypeSymbol s) => c.ReportMultipleHandlers(s));
                                    return false;
                                }
                                mapping[requestMessageSymbol] = null;
                                requestMessage.SetHandler(handler);
                            }
                            else
                            {
                                mapping.Add(requestMessageSymbol, handler);
                            }

                            _requestMessageHandlers.Add(handler);
                        }
                        else
                        {
                            _notificationMessageHandlers.Add(new CommandMessageHandler(typeSymbol, this));
                        }
                    }
                    break;
                case IS_REQUEST:
                    {
                        var responseMessageSymbol = typeInterfaceSymbol.TypeArguments.Length > 0 ?
                            (INamedTypeSymbol)typeInterfaceSymbol.TypeArguments[0] :
                            UnitSymbol;

                        if (IsAlreadyHandledByDerivedInterface(i, 0, typeSymbol, typeInterfaceSymbol))
                        {
                            break;
                        }

                        var messageType = typeInterfaceSymbol.Name.IndexOf('<') == -1 ?
                            typeInterfaceSymbol.Name.Substring(1) :
                            typeInterfaceSymbol.Name.Substring(1, typeInterfaceSymbol.Name.IndexOf('<') - 1);

                        var message = new RequestMessage(typeSymbol, responseMessageSymbol, messageType, this);
                        if (!_requestMessages.Add(message))
                        {
                            // If this symbol has already been added,
                            // the type implements multiple base message types.
                            ReportDiagnostic(typeSymbol, (in GeneratorExecutionContext c, INamedTypeSymbol s) => c.ReportMessageDerivesFromMultipleMessageInterfaces(s));
                            return false;
                        }
                        else
                        {
                            if (mapping.TryGetValue(typeSymbol, out var requestMessageHandlerObj))
                            {
                                mapping[typeSymbol] = null;
                                message.SetHandler((RequestMessageHandler)requestMessageHandlerObj!);
                            }
                            else
                            {
                                mapping.Add(typeSymbol, message);
                            }
                        }
                    }
                    break;
                case IS_NOTIFICATION:
                    {
                        if (!_notificationMessages.Add(new CommandMessage(typeSymbol, this)))
                        {
                            // If this symbol has already been added,
                            // the type implements multiple base message types.
                            ReportDiagnostic(typeSymbol, (in GeneratorExecutionContext c, INamedTypeSymbol s) => c.ReportMessageDerivesFromMultipleMessageInterfaces(s));
                            return false;
                        }
                    }
                    break;
            }

            return true;
        }

        int IsInteresting(INamedTypeSymbol interfaceSymbol)
        {
            var originalInterfaceSymbol = interfaceSymbol.OriginalDefinition;

            for (int i = 0; i < _baseHandlerSymbols.Length; i++)
            {
                var baseSymbol = _baseHandlerSymbols[i];
                if (_symbolComparer.Equals(baseSymbol, originalInterfaceSymbol))
                {
                    return _symbolComparer.Equals(baseSymbol, _notificationHandlerInterfaceSymbol) ?
                        IS_NOTIFICATION_HANDLER :
                        IS_REQUEST_HANDLER;
                }
            }

            for (int i = 0; i < _baseMessageSymbols.Length; i++)
            {
                var baseSymbol = _baseMessageSymbols[i];
                if (_symbolComparer.Equals(baseSymbol, originalInterfaceSymbol))
                {
                    return _symbolComparer.Equals(baseSymbol, _notificationInterfaceSymbol) ?
                        IS_NOTIFICATION :
                        IS_REQUEST;
                }
            }

            return NOT_RELEVANT;
        }

        bool IsAlreadyHandledByDerivedInterface(
            int i,
            int responseTypeArgumentIndex,
            INamedTypeSymbol typeSymbol,
            INamedTypeSymbol typeInterfaceSymbol
        )
        {
            var mightSkip = typeInterfaceSymbol.TypeArguments.Length > responseTypeArgumentIndex &&
                typeInterfaceSymbol.TypeArguments[responseTypeArgumentIndex] is INamedTypeSymbol responseTypeSymbol &&
                _symbolComparer.Equals(responseTypeSymbol, UnitSymbol);

            if (!mightSkip)
                return false;

            for (int j = 0; j < i; j++)
            {
                var prevTypeInterfaceSymbol = typeSymbol.AllInterfaces[j];

                if (prevTypeInterfaceSymbol.ContainingNamespace.Name != Constants.Namespace)
                {
                    continue;
                }

                if (prevTypeInterfaceSymbol.Interfaces.Contains(typeInterfaceSymbol, _symbolComparer))
                {
                    return true;
                }
            }

            return false;
        }
    }

    static bool IsOpenGeneric(INamedTypeSymbol symbol) =>
        symbol.TypeArguments.Length > 0 && symbol.TypeArguments[0] is ITypeParameterSymbol;

    private void TryParseOptions(CancellationToken cancellationToken)
    {
        var compilation = _compilation;

        var attrs = compilation.Assembly.GetAttributes();
        var optionsAttr = attrs.SingleOrDefault(a => a.AttributeClass?.Name == "ApplicationOptions");
        if (optionsAttr is null)
        {
            return;
        }

        var syntaxReference = optionsAttr.ApplicationSyntaxReference;
        if (syntaxReference is null)
        {
            return;
        }

        var semanticModel = compilation.GetSemanticModel(syntaxReference.SyntaxTree);

        if (optionsAttr.ApplicationSyntaxReference?.GetSyntax(cancellationToken) is not AttributeSyntax optionsAttrSyntax || optionsAttrSyntax.ArgumentList is null)
        {
            return;
        }

        foreach (var attrArg in optionsAttrSyntax.ArgumentList.Arguments)
        {
            if (attrArg.NameEquals is null)
            {
                throw new Exception("Error parsing ApplicationOptions");
            }

            var attrFieldName = attrArg.NameEquals.Name.ToString();
            if (attrFieldName == "DefaultServiceLifetime")
            {
                var identifierNameSyntax = (IdentifierNameSyntax)((MemberAccessExpressionSyntax)attrArg.Expression).Name;
                ServiceLifetimeSymbol = (IFieldSymbol)semanticModel.GetSymbolInfo(identifierNameSyntax, cancellationToken).Symbol!;
            }
            else if (attrFieldName == "Namespace")
            {
                var namespaceArg = semanticModel.GetConstantValue(attrArg.Expression, cancellationToken).Value as string;
                if (!string.IsNullOrWhiteSpace(namespaceArg))
                {
                    ApplicationNamespace = namespaceArg!;
                }
            }
        }
    }

    private delegate Diagnostic ReportDiagnosticDelegate<T>(in GeneratorExecutionContext context, T state);
    private void ReportDiagnostic<T>(T state, ReportDiagnosticDelegate<T> del)
    {
        var diagnostic = del(in _context, state);
        _hasErrors |= diagnostic.Severity == DiagnosticSeverity.Error;
    }
}
