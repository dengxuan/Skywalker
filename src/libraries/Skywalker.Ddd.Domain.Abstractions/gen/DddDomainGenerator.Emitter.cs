// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Skywalker.Ddd.Domain.Generators;

public partial class DddDomainGenerator
{
    internal class Emitter
    {
        private const int DefaultStringBuilderCapacity = 1024;
        private static readonly string s_generatedCodeAttribute = @$"[System.CodeDom.Compiler.GeneratedCodeAttribute(""{typeof(Emitter).Assembly.GetName().Name}"", ""{typeof(Emitter).Assembly.GetName().Version}"")]";

        public static void Emit(GeneratorExecutionContext context, IReadOnlyList<DomainServiceClass> domainServiceClasses)
        {
            var builder = new StringBuilder(DefaultStringBuilderCapacity);
            builder.Clear();
            builder.AppendLine("// <auto-generated>");
            builder.AppendLine("//     Generated by the Skywalker.Ddd.Domain.Generators");
            builder.AppendLine("// </auto-generated>");
            builder.AppendLine();
            builder.AppendLine($"using Microsoft.Extensions.DependencyInjection.Extensions;");
            builder.AppendLine();
            builder.AppendLine("#nullable enable");
            builder.AppendLine();
            builder.AppendLine($"namespace Microsoft.Extensions.DependencyInjection;");
            builder.AppendLine();
            builder.AppendLine($"{s_generatedCodeAttribute}");
            builder.AppendLine("public static class DomainIServiceCollectionRepositoryExtensions");
            builder.AppendLine("{");
            builder.AppendLine();
            builder.AppendLine($"\tpublic static DomainServiceBuilder AddGeneralDomainServices(this DomainServiceBuilder builder)");
            builder.AppendLine("\t{");
            foreach (var domainServiceClass in domainServiceClasses)
            {
                foreach (var implInterface in domainServiceClass.ImplInterfaces)
                {
                    builder.AppendLine($"\t\tbuilder.Services.TryAddTransient<{ implInterface.Fullname }, {domainServiceClass.Fullname}>();");
                }
            }
            builder.AppendLine("\t\treturn builder;");
            builder.AppendLine("\t}");
            builder.AppendLine("}");
            context.AddSource("Skywalker.Ddd.DomainServices.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
        }
    }
}
