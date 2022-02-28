// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Skywalker.Extensions.DependencyInjection.Generators;

public partial class DependencyInjectionGenerator
{
    internal class Emitter
    {
        public static void Emit(GeneratorExecutionContext context, IReadOnlyList<DependencyInjectionClass> dependencyInjectionClasses)
        {
            var generatorVersion = typeof(Emitter).Assembly.GetName().Version;
            var file = @"Templates/ModuleInitializer.sbn-cs";
            var template = Template.Parse(EmbeddedResource.GetContent(file), file);
            var output = template.Render(new { GeneratorVersion = generatorVersion, DependencyInjectionClasses = dependencyInjectionClasses }, member => member.Name);
            context.AddSource("ModuleInitializer.g.cs", SourceText.From(output, Encoding.UTF8));
        }
    }
}
