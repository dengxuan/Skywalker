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

    public Task ExecuteNonQueryAsync<TInputDto>(TInputDto inputDto, CancellationToken cancellationToken = default) where TInputDto : IEntityDto
    {
        var handler = _iocResolver.GetRequiredService<IExecuteNonQueryHandlerProvider<TInputDto>>();
        return handler.HandleAsync(inputDto,cancellationToken);
    }

    public Task<TOutputDto?> ExecuteQueryAsync<TOutputDto>(CancellationToken cancellationToken = default) where TOutputDto : IEntityDto
    {
        var handler = _iocResolver.GetRequiredService<IExecuteQueryHandlerProvider<TOutputDto>>();
        return handler.HandleAsync(cancellationToken);
    }

    public Task<TOutputDto?> ExecuteQueryAsync<TInputDto, TOutputDto>(TInputDto inputDto, CancellationToken cancellationToken = default) where TInputDto : IEntityDto where TOutputDto : IEntityDto
    {
        if (inputDto == null)
        {
            throw new ArgumentNullException(nameof(inputDto));
        }

        var handler = _iocResolver.GetRequiredService<IExecuteQueryHandlerProvider<TInputDto, TOutputDto>>();
        return handler.HandleAsync(inputDto, cancellationToken);
    }
}
