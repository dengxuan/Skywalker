// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Skywalker.Identifier.Abstractions;

namespace Skywalker.Identifier;

internal class DefaultIdentifier : IIdentifier
{
    private readonly IServiceProvider _service;

    public DefaultIdentifier(IServiceProvider service) => _service = service;

    public TIdentifier? GetIdentifier<TIdentifier>() where TIdentifier : notnull
    {
        var identifier = _service.GetService<IIdentifierGenerator<TIdentifier>>();
        if (identifier == null)
        {
            return default;
        }
        return identifier.Generate();
    }
}
