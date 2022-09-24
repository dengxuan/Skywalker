// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Application.Dtos.Abstractions;

//public interface IRequestDto : IDto { }

/// <summary>
/// Marker interface to represent a request with a void response
/// </summary>
public interface IRequestDto : IRequestDto<Unit> { }

/// <summary>
/// Marker interface to represent a request with a response
/// </summary>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IRequestDto<out TResponse> : IDto { }
