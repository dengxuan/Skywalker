using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simple.Application.Abstractions;
using Skywalker.Ddd.UnitOfWork;
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
        private readonly IServiceProvider _serviceProvider;

        public SimpleHostedService( IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            ISimpleUserApplicationService simpleUserApplicationService = _serviceProvider.GetRequiredService<ISimpleUserApplicationService>();
           await simpleUserApplicationService.FindUsersAsync();
            cancellationToken.WaitHandle.WaitOne();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
