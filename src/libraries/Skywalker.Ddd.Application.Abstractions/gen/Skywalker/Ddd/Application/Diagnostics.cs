using Microsoft.CodeAnalysis;
using Skywalker.CodeAnalyzers.Analyzers.SourceGenerator;
using System.Runtime.CompilerServices;

namespace Skywalker.Ddd.Application
{
    public static class Diagnostics
    {
        private static long _counter;
        private static readonly HashSet<string> _ids;

        public static IReadOnlyCollection<string> Ids => _ids;

        static Diagnostics()
        {
            _ids = new();

            GenericError = new DiagnosticDescriptor(
                GetNextId(),
                $"{nameof(ApplicationGenerator)} unknown error",
                $"{nameof(ApplicationGenerator)} got unknown error: " + "{0}",
                nameof(ApplicationGenerator),
                DiagnosticSeverity.Error,
                isEnabledByDefault: true);

            MultipleHandlersError = new DiagnosticDescriptor(
                GetNextId(),
                $"{nameof(ApplicationGenerator)} multiple handlers",
                $"{nameof(ApplicationGenerator)} found multiple handlers " + "of message type {0}",
                nameof(ApplicationGenerator),
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

            InvalidHandlerTypeError = new DiagnosticDescriptor(
                GetNextId(),
                $"{nameof(ApplicationGenerator)} invalid handler",
                $"{nameof(ApplicationGenerator)} found invalid handler type " + "{0}",
                nameof(ApplicationGenerator),
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

            OpenGenericRequestHandler = new DiagnosticDescriptor(
                GetNextId(),
                $"{nameof(ApplicationGenerator)} invalid handler",
                $"{nameof(ApplicationGenerator)} found invalid handler type, request/query/command handlers cannot be generic: " + "{0}",
                nameof(ApplicationGenerator),
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

            MessageDerivesFromMultipleMessageInterfaces = new DiagnosticDescriptor(
                GetNextId(),
                $"{nameof(ApplicationGenerator)} invalid message",
                $"{nameof(ApplicationGenerator)} found message that derives from multiple message interfaces: " + "{0}",
                nameof(ApplicationGenerator),
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

            MessageWithoutHandler = new DiagnosticDescriptor(
                GetNextId(),
                $"{nameof(ApplicationGenerator)} message warning",
                $"{nameof(ApplicationGenerator)} found message without any registered handler: " + "{0}",
                nameof(ApplicationGenerator),
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

            static string GetNextId()
            {
                var count = _counter++;
                var id = $"MSG{count.ToString().PadLeft(4, '0')}";
                _ids.Add(id);

                return id;
            }
        }

        private static Diagnostic Report<T>(
            this GeneratorExecutionContext context,
            DiagnosticDescriptor diagnosticDescriptor,
            T arg
        )
        {
            Diagnostic diagnostic;
            if (typeof(T) == typeof(INamedTypeSymbol))
            {
                ref var symbolArg = ref Unsafe.As<T, INamedTypeSymbol>(ref arg);
                var location = symbolArg.Locations.FirstOrDefault(l => l.IsInSource);
                var symbolName = symbolArg.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                diagnostic = Diagnostic.Create(diagnosticDescriptor, location ?? Location.None, symbolName);
            }
            else
            {
                diagnostic = Diagnostic.Create(diagnosticDescriptor, Location.None, arg);
            }
            context.ReportDiagnostic(diagnostic);
            return diagnostic;
        }

        public static readonly DiagnosticDescriptor GenericError;
        internal static Diagnostic ReportGenericError(this GeneratorExecutionContext context, Exception exception) =>
            context.Report(GenericError, exception);

        public static readonly DiagnosticDescriptor MultipleHandlersError;
        internal static Diagnostic ReportMultipleHandlers(this GeneratorExecutionContext context, INamedTypeSymbol messageType) =>
            context.Report(MultipleHandlersError, messageType);

        public static readonly DiagnosticDescriptor InvalidHandlerTypeError;
        internal static Diagnostic ReportInvalidHandlerType(this GeneratorExecutionContext context, INamedTypeSymbol handlerType) =>
            context.Report(InvalidHandlerTypeError, handlerType);

        public static readonly DiagnosticDescriptor OpenGenericRequestHandler;
        internal static Diagnostic ReportOpenGenericRequestHandler(this GeneratorExecutionContext context, INamedTypeSymbol handlerType) =>
            context.Report(OpenGenericRequestHandler, handlerType);

        public static readonly DiagnosticDescriptor MessageDerivesFromMultipleMessageInterfaces;
        internal static Diagnostic ReportMessageDerivesFromMultipleMessageInterfaces(this GeneratorExecutionContext context, INamedTypeSymbol messageType) =>
            context.Report(MessageDerivesFromMultipleMessageInterfaces, messageType);

        public static readonly DiagnosticDescriptor MessageWithoutHandler;
        internal static Diagnostic ReportMessageWithoutHandler(this GeneratorExecutionContext context, INamedTypeSymbol messageType) =>
            context.Report(MessageWithoutHandler, messageType);
    }
}
