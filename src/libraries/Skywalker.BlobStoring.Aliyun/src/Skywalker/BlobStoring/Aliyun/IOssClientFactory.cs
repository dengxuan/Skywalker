using Aliyun.OSS;

namespace Skywalker.BlobStoring.Aliyun;

public interface IOssClientFactory
{
    IOss Create(AliyunBlobProviderConfiguration args);
}
