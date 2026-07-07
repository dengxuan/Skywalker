using Skywalker.Ddd.Domain.Repositories;
using Skywalker.DependencyInjection;
using Skywalker.Sample.Website.Domain;
using Skywalker.Validation;

namespace Skywalker.Sample.Website.Application;

/// <summary>
/// 继承 IApplicationService：静态代理 + UnitOfWorkInterceptor 在方法边界接管
/// 请求级工作单元（由 UoW 中间件预留），事务随方法完成提交。
/// </summary>
public interface IMemberAppService : Ddd.Application.Abstractions.IApplicationService
{
    Task<MemberDto> RegisterAsync(RegisterMemberInput input);
    Task<MemberDto> VerifyAsync(VerifyMemberInput input);
    Task<MemberDto?> FindAsync(Guid id);
}

/// <summary>
/// 会员应用服务：注册时聚合根发出 MemberRegisteredEvent，
/// 事务提交后由事件处理器发送验证码短信与验证邮件。
/// </summary>
[ApplicationService]
internal sealed class MemberAppService(
    IRepository<Member, Guid> members,
    IValidator<RegisterMemberInput> registerValidator) : IMemberAppService
{
    public async Task<MemberDto> RegisterAsync(RegisterMemberInput input)
    {
        var validation = await registerValidator.ValidateAsync(input);
        if (!validation.IsValid)
        {
            throw new Skywalker.Exceptions.SkywalkerValidationException(
                string.Join("; ", validation.Errors.Select(error => error.ErrorMessage)));
        }

        var code = Random.Shared.Next(100000, 999999).ToString();
        var member = new Member(Guid.NewGuid(), input.Email, input.Phone, input.DisplayName, code);

        await members.InsertAsync(member, autoSave: true);
        return ToDto(member);
    }

    public async Task<MemberDto> VerifyAsync(VerifyMemberInput input)
    {
        var member = await members.GetAsync(input.MemberId);
        member.Verify(input.Code);
        await members.UpdateAsync(member, autoSave: true);
        return ToDto(member);
    }

    public async Task<MemberDto?> FindAsync(Guid id)
    {
        var member = await members.FindAsync(id);
        return member is null ? null : ToDto(member);
    }

    private static MemberDto ToDto(Member member) =>
        new(member.Id, member.Email, member.Phone, member.DisplayName, member.IsVerified);
}
