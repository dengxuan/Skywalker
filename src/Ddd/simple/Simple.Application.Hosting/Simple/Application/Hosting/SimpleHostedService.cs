using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simple.Application.Abstractions;
using Skywalker.Ddd.UnitOfWork;
using Skywalker.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Application.Hosting
{
    public class SimpleHostedService : IHostedService
    {
        private readonly ISimpleUserApplicationService _simpleUserApplicationService;

        public SimpleHostedService(ISimpleUserApplicationService simpleUserApplicationService)
        {
            _simpleUserApplicationService = simpleUserApplicationService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    _simpleUserApplicationService.FindUsersAsync();
                }
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
