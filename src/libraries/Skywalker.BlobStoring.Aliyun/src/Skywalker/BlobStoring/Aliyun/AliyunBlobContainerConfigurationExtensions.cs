﻿using Skywalker.BlobStoring.Abstractions;

namespace Skywalker.BlobStoring.Aliyun;


public static class AliyunBlobContainerConfigurationExtensions
{
    public static AliyunBlobProviderConfiguration GetAliyunConfiguration(
        this BlobContainerConfiguration containerConfiguration)
    {
        return new AliyunBlobProviderConfiguration(containerConfiguration);
    }

    public static BlobContainerConfiguration UseAliyun(
        this BlobContainerConfiguration containerConfiguration,
        Action<AliyunBlobProviderConfiguration> aliyunConfigureAction)
    {
        containerConfiguration.ProviderType = typeof(AliyunBlobProvider);
        containerConfiguration.NamingNormalizers.AddIfNotContains(typeof(AliyunBlobNamingNormalizer));

        aliyunConfigureAction(new AliyunBlobProviderConfiguration(containerConfiguration));

        return containerConfiguration;
    }
}
