// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Extensions.Logging.File;

#if NET
internal sealed class FormatterOptionsMonitor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TOptions> : IOptionsMonitor<TOptions>
#elif NETSTANDARD || NETCOREAPP
internal sealed class FormatterOptionsMonitor<TOptions> : IOptionsMonitor<TOptions>
#endif

    where TOptions : FileFormatterOptions
{
    public FormatterOptionsMonitor(TOptions options)
    {
        CurrentValue = options;
    }

    public TOptions Get(string name) => CurrentValue;

    public IDisposable? OnChange(Action<TOptions, string> listener)
    {
        return null;
    }

    public TOptions CurrentValue { get; private set; }
}
