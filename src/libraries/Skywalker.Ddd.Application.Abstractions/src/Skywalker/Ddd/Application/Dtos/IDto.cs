namespace Skywalker.Ddd.Application.Dtos;

public interface IDto { }

public interface ICommand : IDto { }

public interface IQuery<out TResponse> : IDto { }
