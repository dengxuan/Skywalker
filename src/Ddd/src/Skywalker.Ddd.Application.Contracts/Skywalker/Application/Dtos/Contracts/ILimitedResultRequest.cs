namespace Skywalker.Application.Dtos.Contracts
{
    /// <summary>
    /// This interface is defined to standardize to request a limited result.
    /// </summary>
    public interface ILimitedResultRequest : IEntityDto
    {
        /// <summary>
        /// Maximum result count should be returned.
        /// This is generally used to limit result count on paging.
        /// </summary>
        int MaxResultCount { get; set; }
    }
}