// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Skywalker.ExceptionHandler;
using Skywalker.Permissions.Abstractions;
using System.Net.Http.Json;

namespace Skywalker.Permissions.AuthorizeValidation;

internal class RemotePermissionDefinitionManager : IPermissionDefinitionManager
{
    private readonly IMemoryCache _memoryCache;
    private readonly HttpClient _backChannel;
    private readonly Dictionary<string, PermissionDefinition> _permissions = new Dictionary<string, PermissionDefinition>();

    public RemotePermissionDefinitionManager(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
    {
        _backChannel = httpClientFactory.CreateClient(Constants.HttpClientName);
        _memoryCache = memoryCache;
    }

    public async Task<bool> IsGrantedAsync(string name, string providerName, string providerKey)
    {
        var requestUri = QueryHelpers.AddQueryString("connect/checkpermission", new Dictionary<string, string>
        {
            { "name", name },
            { "providerName", providerName },
            { "providerKey", providerKey }
        });
        var response = await _backChannel.GetAsync(requestUri);
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

    public async Task CreatePermissionsAsync(IReadOnlyList<PermissionDefinition> permissionDefinitions)
    {
        await AddPermissionRecursively(permissionDefinitions);
        async Task AddPermissionRecursively(IReadOnlyList<PermissionDefinition> definitions)
        {
            foreach (var definition in definitions)
            {
                _permissions.AddIfNotContains(new KeyValuePair<string, PermissionDefinition>(definition.Name, definition));
                await AddPermissionRecursively(definition.Children);
            }
        }
    }

    public async Task<PermissionDefinition> GetAsync(string name)
    {
        var definition = await GetOrNullAsync(name);
        if (definition == null)
        {
            throw new SkywalkerException($"The permission definition of name {name} not found!");
        }
        return definition;
    }

    public Task<PermissionDefinition?> GetOrNullAsync(string name)
    {
        _permissions.TryGetValue(name, out var definition);
        return Task.FromResult(definition);
    }

    public async Task<IReadOnlyList<PermissionDefinition>> GetPermissionsAsync()
    {
        var definitions = await _memoryCache.GetOrCreateAsync("", async (entry) =>
        {
            return await _backChannel.GetFromJsonAsync<IReadOnlyList<PermissionDefinition>>("");
        });
        return definitions!;
    }

    public async Task<IReadOnlyList<PermissionDefinition>> GetPermissionsAsync(params string[] names)
    {
        var all = await GetPermissionsAsync();
        return all.Where(predicate => names.Contains(predicate.Name)).ToList();
    }
}
