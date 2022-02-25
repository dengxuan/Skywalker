namespace Skywalker.Authorization;

public static class SkywalkerAuthorizationErrorCodes
{
    public const string GivenPolicyHasNotGranted = "Skywalker.Authorization:010001";

    public const string GivenPolicyHasNotGrantedWithPolicyName = "Skywalker.Authorization:010002";

    public const string GivenPolicyHasNotGrantedForGivenResource = "Skywalker.Authorization:010003";

    public const string GivenRequirementHasNotGrantedForGivenResource = "Skywalker.Authorization:010004";

    public const string GivenRequirementsHasNotGrantedForGivenResource = "Skywalker.Authorization:010005";
}
