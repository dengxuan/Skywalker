using Skywalker.BlobStoring.Abstractions;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.BlobStoring.Aliyun;

public class DefaultAliyunBlobNameCalculator : IAliyunBlobNameCalculator, ITransientDependency
{
    public DefaultAliyunBlobNameCalculator()
    {
    }

    public virtual string Calculate(BlobProviderArgs args)
    {
        return args.BlobName;
    }
}
