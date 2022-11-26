// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public class DomainBuilder
{
    /// <summary>
    /// 
    /// </summary>
    public IServiceCollection Services { get; }

    internal DomainBuilder(IServiceCollection services) => Services = services;
}
