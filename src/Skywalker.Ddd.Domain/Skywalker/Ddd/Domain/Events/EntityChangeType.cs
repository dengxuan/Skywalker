namespace Skywalker.Ddd.Domain.Events;

/// <summary>
/// 
/// </summary>
public enum EntityChangeType : byte
{
    /// <summary>
    /// 
    /// </summary>
    Created = 0,

    /// <summary>
    /// 
    /// </summary>
    Updated = 1,

    /// <summary>
    /// 
    /// </summary>
    Deleted = 2
}
