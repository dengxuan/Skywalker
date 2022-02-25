﻿// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Extensions.DependencyInjection;

public interface IPreConfigureServices
{
    void PreConfigureServices(IServiceCollection services);
}