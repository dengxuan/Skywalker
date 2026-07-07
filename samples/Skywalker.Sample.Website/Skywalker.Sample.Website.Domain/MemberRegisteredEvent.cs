namespace Skywalker.Sample.Website.Domain;

/// <summary>
/// 会员注册领域事件：事务提交后发布，处理器负责发送验证码短信与验证邮件。
/// </summary>
public record MemberRegisteredEvent(Guid MemberId, string Email, string Phone, string DisplayName, string VerificationCode);
