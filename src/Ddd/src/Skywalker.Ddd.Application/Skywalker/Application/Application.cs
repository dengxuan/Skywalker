using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Abstractions;
using Skywalker.Application.Dtos.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Application;

public class Application : IApplication
{
    private readonly IServiceProvider _iocResolver;

    public Application(IServiceProvider iocResolver)
    {
        _iocResolver = iocResolver;
    }

    public Task<TOutputDto?> ExecuteAsync<TOutputDto>(CancellationToken cancellationToken = default) where TOutputDto : IEntityDto
    {
        var handler = _iocResolver.GetRequiredService<IExecuteHandlerProvider<TOutputDto>>();
        return handler.HandleAsync(cancellationToken);
    }

    public Task<TOutputDto?> ExecuteAsync<TInputDto, TOutputDto>(TInputDto inputDto, CancellationToken cancellationToken = default) where TInputDto : IEntityDto where TOutputDto : IEntityDto
    {
        if (inputDto == null)
        {
            throw new ArgumentNullException(nameof(inputDto));
        }

        var handler = _iocResolver.GetRequiredService<IExecuteHandlerProvider<TInputDto, TOutputDto>>();
        return handler.HandleAsync(inputDto, cancellationToken);
    }
}
