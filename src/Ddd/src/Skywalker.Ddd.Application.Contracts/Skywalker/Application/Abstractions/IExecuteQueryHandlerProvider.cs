﻿using Microsoft.Extensions.DependencyInjection;
using Skywalker.Application.Dtos.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Application.Abstractions;

public interface IExecuteQueryHandlerProvider<TOutputDto> : IScopedDependency where TOutputDto : IEntityDto
{
    Task<TOutputDto?> HandleAsync(CancellationToken cancellationToken = default);
}

public interface IExecuteQueryHandlerProvider<TInputDto, TOutputDto> : IScopedDependency where TInputDto : IEntityDto where TOutputDto : IEntityDto
{
    Task<TOutputDto?> HandleAsync(TInputDto inputDto, CancellationToken cancellationToken = default);
}
