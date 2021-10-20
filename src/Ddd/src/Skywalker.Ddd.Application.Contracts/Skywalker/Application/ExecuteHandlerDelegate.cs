using Skywalker.Application.Dtos.Contracts;

namespace Skywalker.Application;

public delegate Task<TOutputDto?> ExecuteHandlerDelegate<TOutputDto>(CancellationToken cancellationToken) where TOutputDto : IEntityDto;
