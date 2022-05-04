// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Application.Dtos.Abstractions;
using Skywalker.Extensions.DependencyInjection;
namespace Skywalker.Ddd.Application.Abstractions;

public interface IApplicationHandler : ITransientDependency
{

}

public interface IApplicationHandler<in TRequest> : IApplicationHandler where TRequest : IRequestDto
{
    ValueTask HandleAsync(TRequest request, CancellationToken cancellationToken);
}

public interface IApplicationHandler<in TRequest, TResponse> : IApplicationHandler where TRequest : IRequestDto where TResponse : IResponseDto
{
    ValueTask<TResponse?> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}

