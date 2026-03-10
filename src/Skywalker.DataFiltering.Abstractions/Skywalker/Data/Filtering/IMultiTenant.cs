namespace Skywalker.Data.Filtering;

/// <summary>
/// Interface for entities that support multi-tenancy.
/// </summary>
public interface IMultiTenant
{
    /// <summary>
    /// Gets or sets the tenant ID.
    /// </summary>
    Guid? TenantId { get; set; }
}

