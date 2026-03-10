// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.SimpleStateChecking;

namespace Skywalker.Permissions;

public class RequirePermissionsSimpleBatchStateCheckerModel<TState>
    where TState : IHasSimpleStateCheckers<TState>
{
    public TState State { get; }

    public string[] Permissions { get; }

    public bool RequiresAll { get; }

    public RequirePermissionsSimpleBatchStateCheckerModel(TState state, string[] permissions, bool requiresAll = true)
    {
        state.NotNull(nameof(state));
        Check.NotNullOrEmpty(permissions, nameof(permissions));

        State = state;
        Permissions = permissions;
        RequiresAll = requiresAll;
    }
}
