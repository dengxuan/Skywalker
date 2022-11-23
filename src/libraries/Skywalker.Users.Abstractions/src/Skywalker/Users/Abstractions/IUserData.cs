namespace Skywalker.Users.Abstractions;

/// <summary>
/// 用户数据
/// </summary>
public interface IUserData
{
    /// <summary>
    /// Id
    /// </summary>
    long Id { get; }

    /// <summary>
    /// 用户名
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// 是否活跃
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// 邮箱
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// 邮箱是否确认
    /// </summary>
    bool EmailConfirmed { get; }

    /// <summary>
    /// 电话号码
    /// </summary>
    string? PhoneNumber { get; }

    /// <summary>
    /// 电话是否确认
    /// </summary>
    bool PhoneNumberConfirmed { get; }
}
