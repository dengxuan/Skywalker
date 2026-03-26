// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Permissions.Abstractions;

namespace Skywalker.Permissions.AuthorizeValidation;

/// <summary>
/// 权限同步后台服务，负责从远程服务器同步权限数据到本地缓存
/// </summary>
public class PermissionSyncHostedService : BackgroundService
{
    private readonly ILogger<PermissionSyncHostedService> _logger;
    private readonly IOptions<PermissionValidationOptions> _options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly IPermissionDefinitionManager _definitionManager;

    private string? _permissionGrantsEndpoint;
    private string? _registerEndpoint;
    private string? _currentETag;

    public PermissionSyncHostedService(
        ILogger<PermissionSyncHostedService> logger,
        IOptions<PermissionValidationOptions> options,
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        IPermissionDefinitionManager definitionManager)
    {
        _logger = logger;
        _options = options;
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _definitionManager = definitionManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 等待应用启动完成
        await Task.Delay(1000, stoppingToken);

        try
        {
            // 1. 发现端点
            await DiscoverEndpointsAsync(stoppingToken);

            // 2. 注册权限定义
            await RegisterPermissionsAsync(stoppingToken);

            // 3. 初始同步
            await SyncGrantsAsync(stoppingToken);
            _logger.LogInformation("初始同步权限授权完成");

            // 4. 定期刷新
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_options.Value.RefreshInterval, stoppingToken);
                await CheckAndSyncAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // 正常关闭
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "权限同步服务发生错误");
        }
    }

    private async Task DiscoverEndpointsAsync(CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient(Constants.PermissionHttpClientName);
        var discoveryUrl = "/.well-known/permissions-configuration";

        _logger.LogInformation("发现权限服务端点: {Url}", discoveryUrl);

        var config = await client.GetFromJsonAsync<PermissionsConfigurationResponse>(discoveryUrl, cancellationToken);
        if (config != null)
        {
            _permissionGrantsEndpoint = config.PermissionGrantsEndpoint ?? _options.Value.PermissionGrantsEndpoint;
            _registerEndpoint = config.RegisterEndpoint;
            _logger.LogInformation("发现端点成功: grants={Grants}, register={Register}",
                _permissionGrantsEndpoint, _registerEndpoint);
        }
        else
        {
            _permissionGrantsEndpoint = _options.Value.PermissionGrantsEndpoint;
        }
    }

    private async Task SyncGrantsAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_permissionGrantsEndpoint))
        {
            _logger.LogWarning("同步端点未配置，跳过权限同步");
            return;
        }

        var client = _httpClientFactory.CreateClient(Constants.PermissionHttpClientName);
        var response = await client.GetAsync(_permissionGrantsEndpoint, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var newETag = response.Headers.ETag?.Tag?.Trim('"') ?? string.Empty;
            var grants = await response.Content.ReadFromJsonAsync<List<PermissionGrantInfo>>(cancellationToken: cancellationToken);

            if (grants != null)
            {
                UpdateCache(grants, newETag);
                _currentETag = newETag;
                _logger.LogInformation("同步 {Count} 条权限授权，版本: {Version}", grants.Count, newETag);
            }
        }
        else
        {
            _logger.LogWarning("同步权限数据失败: {Status}", response.StatusCode);
        }
    }

    private void UpdateCache(List<PermissionGrantInfo> grants, string version)
    {
        var grantDict = new Dictionary<string, HashSet<(string, string)>>();

        foreach (var grant in grants)
        {
            if (!grantDict.TryGetValue(grant.Name, out var set))
            {
                set = new HashSet<(string, string)>();
                grantDict[grant.Name] = set;
            }
            set.Add((grant.ProviderName ?? string.Empty, grant.ProviderKey ?? string.Empty));
        }

        _cache.Set(InMemoryPermissionValidator.GrantsCacheKey, grantDict);
        _cache.Set(Constants.VersionCacheKey, version);
    }

    private async Task CheckAndSyncAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_permissionGrantsEndpoint))
        {
            return;
        }

        var client = _httpClientFactory.CreateClient(Constants.PermissionHttpClientName);

        // 使用 HEAD 请求检查 ETag
        var request = new HttpRequestMessage(HttpMethod.Head, _permissionGrantsEndpoint);
        if (!string.IsNullOrEmpty(_currentETag))
        {
            request.Headers.IfNoneMatch.Add(new System.Net.Http.Headers.EntityTagHeaderValue($"\"{_currentETag}\""));
        }

        var response = await client.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
        {
            _logger.LogDebug("权限数据未变更");
            return;
        }

        if (response.IsSuccessStatusCode)
        {
            var serverETag = response.Headers.ETag?.Tag?.Trim('"');
            if (serverETag != _currentETag)
            {
                _logger.LogInformation("检测到权限数据变更，重新同步");
                await SyncGrantsAsync(cancellationToken);
            }
        }
    }

    private async Task RegisterPermissionsAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_registerEndpoint))
        {
            _logger.LogWarning("注册端点未配置，跳过权限注册");
            return;
        }

        var definitions = await _definitionManager.GetPermissionsAsync();
        if (!definitions.Any())
        {
            _logger.LogInformation("没有权限定义需要注册");
            return;
        }

        var client = _httpClientFactory.CreateClient(Constants.PermissionHttpClientName);
        var request = new HttpRequestMessage(HttpMethod.Post, _registerEndpoint)
        {
            Content = JsonContent.Create(definitions.Select(d => new
            {
                d.Name,
                d.DisplayName,
                d.IsEnabled,
                Children = GetChildrenNames(d)
            }))
        };

        // 添加服务间认证
        if (!string.IsNullOrEmpty(_options.Value.Secret))
        {
            request.Headers.Add("X-Secret", _options.Value.Secret);
        }

        var response = await client.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("注册 {Count} 个权限定义成功", definitions.Count);
        }
        else
        {
            _logger.LogWarning("注册权限定义失败: {Status}", response.StatusCode);
        }
    }

    private static IEnumerable<string> GetChildrenNames(PermissionDefinition definition)
    {
        return definition.Children.Select(c => c.Name);
    }
}

internal class PermissionsConfigurationResponse
{
    public string? PermissionGrantsEndpoint { get; set; }
    public string? RegisterEndpoint { get; set; }
}
