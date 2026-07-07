using System.Security.Claims;
using Skywalker.Security.Claims;

namespace Skywalker.Sample.Website.Web.Infrastructure;

/// <summary>
/// 让 Security 家族的 ICurrentUser / 权限检查读取 HttpContext.User。
/// </summary>
public sealed class HttpContextCurrentPrincipalAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentPrincipalAccessor
{
    private static readonly AsyncLocal<ClaimsPrincipal?> s_override = new();

    public ClaimsPrincipal? Principal => s_override.Value ?? httpContextAccessor.HttpContext?.User;

    public IDisposable Change(ClaimsPrincipal principal)
    {
        var previous = s_override.Value;
        s_override.Value = principal;
        return new DisposeAction(() => s_override.Value = previous);
    }

    private sealed class DisposeAction(Action action) : IDisposable
    {
        public void Dispose() => action();
    }
}

/// <summary>
/// 演示认证中间件：从 X-Demo-User / X-Demo-Roles 请求头构造 ClaimsPrincipal。
/// 真实站点替换为 JWT/Cookie 等标准认证即可，下游组件（ICurrentUser/权限）不感知差异。
/// </summary>
public sealed class DemoAuthenticationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var user = context.Request.Headers["X-Demo-User"].ToString();
        if (!string.IsNullOrEmpty(user))
        {
            var claims = new List<Claim>
            {
                new(SkywalkerClaimTypes.UserId, user),
                new(SkywalkerClaimTypes.UserName, user),
            };

            foreach (var role in context.Request.Headers["X-Demo-Roles"].ToString()
                         .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                claims.Add(new Claim(SkywalkerClaimTypes.Role, role));
            }

            context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: "Demo"));
        }

        await next(context);
    }
}
