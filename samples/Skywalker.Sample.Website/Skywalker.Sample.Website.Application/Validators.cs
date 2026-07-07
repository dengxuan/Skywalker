using FluentValidation;

namespace Skywalker.Sample.Website.Application;

/// <summary>
/// FluentValidation 验证器：AddFluentValidation&lt;T&gt;() 扫描本程序集自动注册，
/// 同时桥接为 Skywalker.Validation.IValidator&lt;T&gt;。
/// </summary>
public sealed class RegisterMemberInputValidator : AbstractValidator<RegisterMemberInput>
{
    public RegisterMemberInputValidator()
    {
        RuleFor(input => input.Email).NotEmpty().EmailAddress();
        RuleFor(input => input.Phone).NotEmpty().Matches(@"^\+?\d{6,15}$");
        RuleFor(input => input.DisplayName).NotEmpty().MaximumLength(32);
    }
}

public sealed class PlaceOrderInputValidator : AbstractValidator<PlaceOrderInput>
{
    public PlaceOrderInputValidator()
    {
        RuleFor(input => input.CustomerEmail).NotEmpty().EmailAddress();
        RuleFor(input => input.Items).NotEmpty();
        RuleForEach(input => input.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0).LessThanOrEqualTo(99);
        });
    }
}
