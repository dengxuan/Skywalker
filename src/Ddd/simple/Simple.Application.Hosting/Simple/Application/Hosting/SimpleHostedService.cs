using Microsoft.Extensions.Hosting;
using Simple.Application.Abstractions;
using Simple.Application.Users;
using Skywalker.Ddd.Queries.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Application.Hosting
{
    public class SimpleHostedService : IHostedService
    {
        private readonly ISearcher _searcher;

        public SimpleHostedService(ISearcher searcher)
        {
            _searcher = searcher;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                List<UserDto> userDtos = await _searcher.SearchAsync<UserQuery, List<UserDto>>(new UserQuery { Name = "" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            cancellationToken.WaitHandle.WaitOne();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
