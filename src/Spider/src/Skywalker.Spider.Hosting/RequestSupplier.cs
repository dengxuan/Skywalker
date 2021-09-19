using Skywalker.Extensions.HtmlAgilityPack;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Http;
using Skywalker.Spider.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Spider.Hosting
{
    public class Middleware
    {
        private readonly PipelineDelegate _next;

        public Middleware(PipelineDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(PipelineContext context)
        {
            string json = Encoding.UTF8.GetString(context.Response!.Content!.Bytes);
            Console.WriteLine(json);
            return _next(context);
        }
    }

    public class RequestSupplier : IRequestSupplier
    {
        public Task<IEnumerable<Request>> GetAllListAsync(CancellationToken cancellationToken)
        {
            string requestUri = "https://www.ti.com.cn/productmodel/BQ24195L/orderables?locale=zh-CN&orderable=BQ24195LRGER";
            return Task.FromResult(Enumerable.Repeat(new Request(requestUri) { Downloader = "Http" }, 1));
        }
    }
}
