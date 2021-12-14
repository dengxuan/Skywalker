// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Extensions.Logging.File;

#if NET5_0_OR_GREATER
internal sealed class FormatterOptionsMonitor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TOptions> : IOptionsMonitor<TOptions>
#elif NETSTANDARD
internal sealed class FormatterOptionsMonitor<TOptions> : IOptionsMonitor<TOptions>
#endif

    where TOptions : FileFormatterOptions
{
    private readonly TOptions _options;
    public FormatterOptionsMonitor(TOptions options)
    {
        _options = options;
    }

    public TOptions Get(string name) => _options;

    public IDisposable? OnChange(Action<TOptions, string> listener)
    {
        return null;
    }

    public TOptions CurrentValue => _options;
}
