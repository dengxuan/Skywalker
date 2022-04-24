// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Application.Dtos.Abstractions;

namespace Skywalker.Ddd.Application.Abstractions;

public interface IApplicationHandler<in TRequest> where TRequest : IRequestDto
{
    ValueTask HandleAsync(TRequest request, CancellationToken cancellationToken);
}

public interface IApplicationHandler<in TRequest, TResponse> where TRequest : IRequestDto where TResponse : IResponseDto
{
    ValueTask<TResponse?> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}

