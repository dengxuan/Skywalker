using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Sample.Website.Domain;

/// <summary>
/// 会员聚合根：注册 → 发送验证码（短信/邮件）→ 验证激活。
/// </summary>
public class Member : AggregateRoot<Guid>
{
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? VerificationCode { get; private set; }
    public bool IsVerified { get; private set; }

    protected Member() { }

    public Member(Guid id, string email, string phone, string displayName, string verificationCode) : base(id)
    {
        Email = email;
        Phone = phone;
        DisplayName = displayName;
        VerificationCode = verificationCode;
        IsVerified = false;

        AddDistributedEvent(new MemberRegisteredEvent(Id, Email, Phone, DisplayName, verificationCode));
    }

    public void Verify(string code)
    {
        if (IsVerified)
        {
            return;
        }

        if (!string.Equals(VerificationCode, code, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("验证码不正确");
        }

        IsVerified = true;
        VerificationCode = null;
    }
}
