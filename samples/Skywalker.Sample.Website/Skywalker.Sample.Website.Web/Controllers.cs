using Microsoft.AspNetCore.Mvc;
using Skywalker.Localization;
using Skywalker.Sample.Website.Application;
using Skywalker.Security.Users;
using Skywalker.Settings.Abstractions;

namespace Skywalker.Sample.Website.Web;

[ApiController]
public sealed class SiteController(
    ISettingProvider settings,
    IStringLocalizer<WebsiteResource> localizer,
    ICurrentUser currentUser) : ControllerBase
{
    [HttpGet("/")]
    public async Task<IActionResult> GetAsync() => Ok(new
    {
        site = await settings.GetOrNullAsync(WebsiteSettings.SiteName),
        welcome = localizer["Welcome"].Value,
    });

    [HttpGet("/api/me")]
    public IActionResult Me() => Ok(new
    {
        isAuthenticated = currentUser.IsAuthenticated,
        userName = currentUser.Username,
        roles = currentUser.Roles,
    });
}

[ApiController]
[Route("api/members")]
public sealed class MembersController(IMemberAppService members) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterMemberInput input) => Ok(await members.RegisterAsync(input));

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyAsync(VerifyMemberInput input) => Ok(await members.VerifyAsync(input));
}

[ApiController]
[Route("api/products")]
public sealed class ProductsController(IProductAppService products) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetListAsync() => Ok(await products.GetListAsync());

    [HttpPut("{id:guid}/price")]
    public async Task<IActionResult> UpdatePriceAsync(Guid id, UpdatePriceInput input) => Ok(await products.UpdatePriceAsync(id, input));
}

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(IOrderAppService orders) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PlaceAsync(PlaceOrderInput input) => Ok(await orders.PlaceAsync(input));

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> ConfirmAsync(Guid id) => Ok(await orders.ConfirmAsync(id));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> FindAsync(Guid id) =>
        await orders.FindAsync(id) is { } order ? Ok(order) : NotFound();
}
