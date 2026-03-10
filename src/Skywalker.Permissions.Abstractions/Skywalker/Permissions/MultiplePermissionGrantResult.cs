// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Permissions;

public class MultiplePermissionGrantResult
{
    public bool AllGranted
    {
        get
        {
            return Result.Values.All(x => x == PermissionGrantResult.Granted);
        }
    }

    public bool AllProhibited
    {
        get
        {
            return Result.Values.All(x => x == PermissionGrantResult.Prohibited);
        }
    }

    public Dictionary<string, PermissionGrantResult> Result { get; }

    public MultiplePermissionGrantResult()
    {
        Result = new Dictionary<string, PermissionGrantResult>();
    }

    public MultiplePermissionGrantResult(string[] names, PermissionGrantResult grantResult = PermissionGrantResult.Undefined)
    {
        Result = new Dictionary<string, PermissionGrantResult>();

        foreach (var name in names)
        {
            Result.Add(name, grantResult);
        }
    }
}
