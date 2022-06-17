using Microsoft.AspNetCore.WebUtilities;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions.AuthorizeValidation;

internal class RemotePermissionValidator : IPermissionValidator
{
    private readonly HttpClient _backendChannel;

    public RemotePermissionValidator(IHttpClientFactory httpClientFactory)
    {
        _backendChannel = httpClientFactory.CreateClient();
    }

    public async Task<bool> IsGrantedAsync(string name, string providerName, string providerKey)
    {
        var requestUri = QueryHelpers.AddQueryString("connect/checkpermission", new Dictionary<string, string>
        {
            { "name", name },
            { "providerName", providerName },
            { "providerKey", providerKey }
        });
        var response = await _backendChannel.GetAsync(requestUri);
        return response.IsSuccessStatusCode;
    }

    public async Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names, string providerName, string providerKey)
    {
        var result = new MultiplePermissionGrantResult(names);
        foreach (var name in names)
        {
            var isGranted = await IsGrantedAsync(name, providerName, providerKey);
            result.Result[name] = isGranted ? PermissionGrantResult.Granted : PermissionGrantResult.Prohibited;
        }
        return result;
    }
}
