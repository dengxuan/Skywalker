using Skywalker.BlobStoring.Abstractions;

namespace Skywalker.BlobStoring.Aliyun;

public interface IAliyunBlobNameCalculator
{
    string Calculate(BlobProviderArgs args);
}
