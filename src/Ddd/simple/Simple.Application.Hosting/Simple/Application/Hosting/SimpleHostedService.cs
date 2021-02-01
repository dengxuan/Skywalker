using Microsoft.Extensions.Hosting;
using Simple.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Application.Hosting
{
    public class SimpleHostedService : IHostedService
    {
        private readonly ISimpleUserApplicationService _searcher;

        public SimpleHostedService(ISimpleUserApplicationService searcher)
        {
            _searcher = searcher;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                List<UserDto> userDtos = await _searcher.FindUsersAsync();
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
