﻿namespace Skywalker.BlobStoring.Abstractions;

public static class BlobContainerConfigurationProviderExtensions
{
    public static BlobContainerConfiguration Get<TContainer>(
        this IBlobContainerConfigurationProvider configurationProvider)
    {
        return configurationProvider.Get(BlobContainerNameAttribute.GetContainerName<TContainer>());
    }
}
