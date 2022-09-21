// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Skywalker.Ddd.Application.Generators;

public partial class DddApplicationGenerator
{
    internal class Builder
    {
        /// <summary>
        /// 接口符号
        /// </summary>
        private readonly INamedTypeSymbol _applicationService;

        /// <summary>
        /// HttpApi代码构建器
        /// </summary>
        /// <param name="applicationService"></param>
        public Builder(INamedTypeSymbol applicationService)
        {
            _applicationService = applicationService;
        }

        public IReadOnlyList<DbContextClass> Build()
        {
            var results = new List<DbContextClass>();
            foreach (var item in _applicationService.GetMembers())
            {
                if(item is IMethodSymbol methodSymbol)
                {
                    var @namespace = _applicationService.ContainingNamespace.ToDisplayString();
                    var handlers = new DbContextClass(@namespace, _applicationService.Name, methodSymbol.Name, methodSymbol.ReturnType, methodSymbol.Parameters);
                    results.Add(handlers);
                }
            }
            return results;
        }
    }


    /// <summary>
    /// A DbContext struct holding a bunch of DbContext properties.
    /// </summary>
    internal record struct DbContextClass(string Namespace, string Name, string MethodName, ITypeSymbol ReturnSymbol, ImmutableArray<IParameterSymbol> ArgumentSymbols);
}
