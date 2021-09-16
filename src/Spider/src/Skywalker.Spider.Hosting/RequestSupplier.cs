using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Spider.Hosting
{
    public class RequestSupplier : IRequestSupplier
    {
        public Task<IEnumerable<Request>> GetAllListAsync(CancellationToken cancellationToken)
        {
            string requestUri = "https://www.ti.com.cn/productmodel/BQ24195L/orderables?locale=zh-CN&orderable=BQ24195LRGER";
            return Task.FromResult(Enumerable.Repeat(new Request(requestUri), 1));
        }
    }
}
