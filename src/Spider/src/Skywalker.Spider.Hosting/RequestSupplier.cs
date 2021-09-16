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
            return Task.FromResult(Enumerable.Repeat(new Request("http://www.baidu.com/"), 1));
        }
    }
}
