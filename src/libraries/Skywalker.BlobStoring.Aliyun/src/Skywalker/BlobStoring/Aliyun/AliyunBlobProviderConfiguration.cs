using System;
using Skywalker.BlobStoring.Abstractions;

namespace Skywalker.BlobStoring.Aliyun;

/// <summary>
/// Sub-account access to OSS or STS temporary authorization to access OSS
/// </summary>
public class AliyunBlobProviderConfiguration
{
    /// <summary>
    /// 云账号AccessKey是访问阿里云API的密钥,具有该账户完全的权限,请你务必妥善保管!强烈建议遵循阿里云安全最佳实践,使用RAM子用户AccessKey来进行API调用.
    /// </summary>
    public string AccessKeyId
    {
        get => _containerConfiguration.GetConfiguration<string>(AliyunBlobProviderConfigurationNames.AccessKeyId);
        set => _containerConfiguration.SetConfiguration(AliyunBlobProviderConfigurationNames.AccessKeyId, Check.NotNullOrWhiteSpace(value, nameof(value)));
    }

    /// <summary>
    /// 云账号AccessKey是访问阿里云API的密钥,具有该账户完全的权限,请你务必妥善保管!强烈建议遵循阿里云安全最佳实践,使用RAM子用户AccessKey来进行API调用.
    /// </summary>
    public string AccessKeySecret
    {
        get => _containerConfiguration.GetConfiguration<string>(AliyunBlobProviderConfigurationNames.AccessKeySecret);
        set => _containerConfiguration.SetConfiguration(AliyunBlobProviderConfigurationNames.AccessKeySecret, Check.NotNullOrWhiteSpace(value, nameof(value)));
    }

    /// <summary>
    /// Endpoint表示OSS对外服务的访问域名. 访问域名和数据中心
    /// </summary>
    public string Endpoint
    {
        get => _containerConfiguration.GetConfiguration<string>(AliyunBlobProviderConfigurationNames.Endpoint);
        set => _containerConfiguration.SetConfiguration(AliyunBlobProviderConfigurationNames.Endpoint, Check.NotNullOrWhiteSpace(value, nameof(value)));
    }

    /// <summary>
    /// 是否使用STS临时授权访问OSS,默认false. STS临时授权访问OSS
    /// </summary>
    public bool UseSecurityTokenService
    {
        get => _containerConfiguration.GetConfigurationOrDefault(AliyunBlobProviderConfigurationNames.UseSecurityTokenService, false);
        set => _containerConfiguration.SetConfiguration(AliyunBlobProviderConfigurationNames.UseSecurityTokenService, value);
    }

    /// <summary>
    /// STS服务的接入地址,每个地址的功能都相同,请尽量在同地域进行调用. 接入地址
    /// </summary>
    public string RegionId
    {
        get => _containerConfiguration.GetConfiguration<string>(AliyunBlobProviderConfigurationNames.RegionId);
        set => _containerConfiguration.SetConfiguration(AliyunBlobProviderConfigurationNames.RegionId, value);
    }

    /// <summary>
    /// STS所需角色ARN.
    /// </summary>
    public string RoleArn
    {
        get => _containerConfiguration.GetConfiguration<string>(AliyunBlobProviderConfigurationNames.RoleArn);
        set => _containerConfiguration.SetConfiguration(AliyunBlobProviderConfigurationNames.RoleArn, value);
    }

    /// <summary>
    /// 用来标识临时访问凭证的名称,建议使用不同的应用程序用户来区分.
    /// </summary>
    public string RoleSessionName
    {
        get => _containerConfiguration.GetConfiguration<string>(AliyunBlobProviderConfigurationNames.RoleSessionName);
        set => _containerConfiguration.SetConfiguration(AliyunBlobProviderConfigurationNames.RoleSessionName, value);
    }

    /// <summary>
    /// 设置临时访问凭证的有效期,单位是s,最小为900,最大为3600.
    /// </summary>
    public int DurationSeconds
    {
        get => _containerConfiguration.GetConfigurationOrDefault(AliyunBlobProviderConfigurationNames.DurationSeconds, 0);
        set => _containerConfiguration.SetConfiguration(AliyunBlobProviderConfigurationNames.DurationSeconds, value);
    }

    /// <summary>
    /// 在扮演角色的时候额外添加的权限限制. 请参见基于RAM Policy的权限控制.
    /// </summary>
    public string Policy
    {
        get => _containerConfiguration.GetConfiguration<string>(AliyunBlobProviderConfigurationNames.Policy);
        set => _containerConfiguration.SetConfiguration(AliyunBlobProviderConfigurationNames.Policy, value);
    }

    /// <summary>
    /// 你可以在aliyun中指定容器名称. 如果没有指定它将使用 BlobContainerName 属性定义的BLOB容器的名称(请参阅BLOB存储文档). 请注意Aliyun有一些命名容器的规则,容器名称必须是有效的DNS名称,符合以下命名规则:
    /// 只能包含小写字母,数字和短横线(-)
    /// 必须以小写字母和数字开头和结尾
    /// Bucket名称的长度限制在3到63个字符之间
    /// </summary>
    public string ContainerName
    {
        get => _containerConfiguration.GetConfigurationOrDefault<string>(AliyunBlobProviderConfigurationNames.ContainerName, string.Empty)!;
        set => _containerConfiguration.SetConfiguration(AliyunBlobProviderConfigurationNames.ContainerName, value);
    }

    /// <summary>
    /// 默认值为 false, 如果aliyun中不存在容器, AliyunBlobProvider 将尝试创建它.
    /// </summary>
    public bool CreateContainerIfNotExists
    {
        get => _containerConfiguration.GetConfigurationOrDefault(AliyunBlobProviderConfigurationNames.CreateContainerIfNotExists, false);
        set => _containerConfiguration.SetConfiguration(AliyunBlobProviderConfigurationNames.CreateContainerIfNotExists, value);
    }

    private readonly string _temporaryCredentialsCacheKey;

    /// <summary>
    /// STS凭证缓存Key,默认Guid.NewGuid().ToString("N").
    /// </summary>
    public string TemporaryCredentialsCacheKey
    {
        get => _containerConfiguration.GetConfigurationOrDefault(AliyunBlobProviderConfigurationNames.TemporaryCredentialsCacheKey, _temporaryCredentialsCacheKey)!;
        set => _containerConfiguration.SetConfiguration(AliyunBlobProviderConfigurationNames.TemporaryCredentialsCacheKey, value);
    }

    private readonly BlobContainerConfiguration _containerConfiguration;

    public AliyunBlobProviderConfiguration(BlobContainerConfiguration containerConfiguration)
    {
        _containerConfiguration = containerConfiguration;
        _temporaryCredentialsCacheKey = Guid.NewGuid().ToString("N");
    }
}
