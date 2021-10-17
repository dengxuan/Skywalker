using Microsoft.Extensions.Hosting;
using Simple.Application.Abstractions;
using Skywalker.Application.Abstractions;
using Skywalker.Application.Dtos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Application.Hosting
{
    public class SimpleHostedService : IHostedService
    {
        private readonly IApplication _searcher;

        public SimpleHostedService(IApplication searcher)
        {
            _searcher = searcher;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                PagedResultDto<UserOutputDto>? userDtos = await _searcher.ExecuteQueryAsync<UserInputDto, PagedResultDto<UserOutputDto>>(new UserInputDto(""), cancellationToken);
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
