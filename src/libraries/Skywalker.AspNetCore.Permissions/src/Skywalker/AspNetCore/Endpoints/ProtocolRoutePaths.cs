// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.AspNetCore.Endpoints;

internal class ProtocolRoutePaths
{
    public const string PathPrefix = "connect";

    public const string VerifyPermission = $"{PathPrefix}/verifypermission";

    public const string PushPermissions = $"{PathPrefix}/pushpermissions";
    
    public const string PullPermissions = $"{PathPrefix}/pullpermissions";
}
