using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Dtos.Contracts;
using Skywalker.Ddd.Application.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.Application
{

    public class ApplicationHandlerProvider<TOutputDto> : IApplicationHandlerProvider<TOutputDto>
        where TOutputDto : IEntityDto
    {
        private readonly IServiceProvider _serviceProvider;

        public ApplicationHandlerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TOutputDto?> HandleAsync(CancellationToken cancellationToken)
        {
            IApplicationHandler<TOutputDto> queryHandler = _serviceProvider.GetRequiredService<IApplicationHandler<TOutputDto>>();
            Task<TOutputDto?> Handler() => queryHandler.HandleAsync(cancellationToken);

            return _serviceProvider.GetServices<IApplicationPipelineBehavior<TOutputDto>>()
                                   .Reverse()
                                   .Aggregate((ApplicationHandlerDelegate<TOutputDto>)Handler, (next, pipeline) => () => pipeline.HandleAsync(next, cancellationToken))();
        }
    }

    public class DefaultApplicationHandlerProvider<TInputDto, TOutputDto> : IApplicationHandlerProvider<TInputDto, TOutputDto>
        where TInputDto : IEntityDto
        where TOutputDto : IEntityDto
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultApplicationHandlerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TOutputDto?> HandleAsync(TInputDto inputDto, CancellationToken cancellationToken)
        {
            IApplicationHandler<TInputDto, TOutputDto> queryHandler = _serviceProvider.GetRequiredService<IApplicationHandler<TInputDto, TOutputDto>>();
            Task<TOutputDto?> Handler() => queryHandler.HandleAsync(inputDto, cancellationToken);

            return _serviceProvider.GetServices<IApplicationPipelineBehavior<TInputDto, TOutputDto>>()
                                   .Reverse()
                                   .Aggregate((ApplicationHandlerDelegate<TOutputDto>)Handler, (next, pipeline) => () => pipeline.HandleAsync(inputDto, next, cancellationToken))();
        }
    }
}
