﻿using System.Threading;


namespace Skywalker.BlobStoring.Abstractions;

public class BlobProviderGetArgs : BlobProviderArgs
{
    public BlobProviderGetArgs(
         string containerName,
         BlobContainerConfiguration configuration,
         string blobName,
        CancellationToken cancellationToken = default)
        : base(
            containerName,
            configuration,
            blobName,
            cancellationToken)
    {
    }
}
