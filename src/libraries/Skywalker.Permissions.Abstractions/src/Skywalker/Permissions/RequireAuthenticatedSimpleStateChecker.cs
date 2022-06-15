// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Skywalker.Extensions.SimpleStateChecking;
using Skywalker.Security.Users;

namespace Skywalker.Permissions;

public class RequireAuthenticatedSimpleStateChecker<TState> : ISimpleStateChecker<TState> where TState : IHasSimpleStateCheckers<TState>
{
    public Task<bool> IsEnabledAsync(SimpleStateCheckerContext<TState> context)
    {
        return Task.Run(() =>
        {
            var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();
            return currentUser.IsAuthenticated;
        });
    }
}
