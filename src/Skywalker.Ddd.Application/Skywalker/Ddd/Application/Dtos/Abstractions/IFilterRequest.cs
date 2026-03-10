
namespace Skywalker.Ddd.Application.Dtos.Abstractions;

/// <summary>
/// Filtered request interface
/// </summary>
public interface IFilteredRequest : IRequestDto
{
    /// <summary>
    /// The filter condition
    /// </summary>
    string? Filter { get; init; }
}