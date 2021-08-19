using Skywalker.Splider.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Splider.Abstractions;

/// <summary>
/// 请求供应接口
/// </summary>
public interface IRequestSupplier
{
    /// <summary>
    /// 运行请求供应
    /// </summary>
    Task<IEnumerable<Request>> GetAllListAsync(CancellationToken cancellationToken);
}
