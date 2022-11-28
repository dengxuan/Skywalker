// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.Application;
using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Ddd.Application.Pipeline;
using Skywalker.Ddd.Application.Pipeline.Abstractions;

namespace Microsoft.Extensions.DependencyInjnection;

public static class ApplicationDddBuilderExtensions
{
    public static DddBuilder AddPipeline(this DddBuilder builder)
    {
        builder.Services.TryAddSingleton<IApplication, DefaultApplication>();
        builder.Services.TryAddSingleton<IPipelineChainBuilder, PipelineChainBuilder>();
        return builder;
    }
}
