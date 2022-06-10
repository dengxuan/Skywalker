// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.PermissionsEvaluator.Abstractions;

public abstract class PermissionValueProvider : IPermissionValueProvider
{
    public abstract string Name { get; }

    protected IPermissionValidator Validator { get; }

    protected PermissionValueProvider(IPermissionValidator validator)
    {
        Validator = validator;
    }

    public abstract Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context);

    public abstract Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context);
}
