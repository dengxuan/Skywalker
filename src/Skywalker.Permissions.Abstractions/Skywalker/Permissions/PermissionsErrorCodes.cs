// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Permissions;

public static class PermissionsErrorCodes
{
    public const string GivenPolicyHasNotGranted = "Skywalker.Permissions:010001";

    public const string GivenPolicyHasNotGrantedWithPolicyName = "Skywalker.Permissions:010002";

    public const string GivenPolicyHasNotGrantedForGivenResource = "Skywalker.Permissions:010003";

    public const string GivenRequirementHasNotGrantedForGivenResource = "Skywalker.Permissions:010004";

    public const string GivenRequirementsHasNotGrantedForGivenResource = "Skywalker.Permissions:010005";
}
