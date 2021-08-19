using Microsoft.Extensions.Hosting;
using Simple.Application.Abstractions;
using Simple.Application.Users;
using Skywalker.Application.Dtos;
using Skywalker.Ddd.Application.Abstractions;
using System;
using System.Collections.Generic;
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
                PagedResultDto<UserOutputDto>? userDtos = await _searcher.ExecuteAsync<UserInputDto, PagedResultDto<UserOutputDto>>(new UserInputDto { Name = "" });
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
