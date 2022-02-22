// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Application.Commands;
using Skywalker.Ddd.Application.Commands.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationCommandsIServiceCollectionExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        services.AddSingleton<ICommander, DefaultCommander>();
        services.AddSingleton(typeof(ICommandPublisher<>), typeof(DefaultCommandPublisher<>));
        services.AddSingleton(typeof(ICommandHandlerProvider<,>), typeof(DefaultCommandHandlerProvider<,>));
        return services;
    }
}
