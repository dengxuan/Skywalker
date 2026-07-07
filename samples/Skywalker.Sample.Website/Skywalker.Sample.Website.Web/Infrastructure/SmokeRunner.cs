using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Skywalker.Sample.Website.Application;
using Skywalker.Sample.Website.Domain;

namespace Skywalker.Sample.Website.Web.Infrastructure;

/// <summary>
/// CI 自验证：对进程内站点按真实用户路径打一遍请求，
/// 断言 注册→短信/邮件→浏览(缓存)→权限→下单(验证/设置/代理)→确认→事件邮件 全链路。
/// </summary>
public static class SmokeRunner
{
    public static async Task<int> RunAsync(WebApplication app)
    {
        var baseAddress = app.Urls.First();
        using var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
        // 默认请求原始契约（关闭 AjaxResponse 包装）；信封行为单独断言
        client.DefaultRequestHeaders.Add("x-wrap-result", "false");
        var failures = new List<string>();

        void Ensure(bool condition, string check)
        {
            if (!condition)
            {
                failures.Add(check);
                Console.Error.WriteLine($"SMOKE FAIL: {check}");
            }
            else
            {
                Console.WriteLine($"smoke ok: {check}");
            }
        }

        // 0. 事件总线连通性（直接调用处理器，暴露被消费循环吞掉的异常）
        try
        {
            var probeHandler = ActivatorUtilities.CreateInstance<MemberRegisteredEventHandler>(app.Services);
            await probeHandler.HandleEventAsync(new MemberRegisteredEvent(Guid.Empty, "probe@example.com", "+8613800000000", "probe", "000000"));
            Ensure(NotificationLog.Entries.Contains($"member-registered:{Guid.Empty}"), "handler executes (direct invocation)");
        }
        catch (Exception probeException)
        {
            Ensure(false, $"handler executes (direct invocation): {probeException}");
        }

        // 1. 站点信息：Settings 默认值 + 英文本地化
        var home = await client.GetFromJsonAsync<JsonElement>("/");
        Ensure(home.GetProperty("site").GetString() == "Skywalker Shop", "settings default value served");
        Ensure(home.GetProperty("welcome").GetString() == "Welcome to Skywalker Shop", $"localization en (actual: {home.GetProperty("welcome").GetString()})");

        // 2. 中文本地化（Accept-Language）
        using (var request = new HttpRequestMessage(HttpMethod.Get, "/"))
        {
            request.Headers.Add("Accept-Language", "zh-CN");
            var zh = await (await client.SendAsync(request)).Content.ReadFromJsonAsync<JsonElement>();
            Ensure(zh.GetProperty("welcome").GetString() == "欢迎光临 Skywalker 商城", $"localization zh-CN (actual: {zh.GetProperty("welcome").GetString()})");
        }

        // 2b. AjaxResponse 信封：不带 x-wrap-result:false 时错误响应被包装为 200 + 信封
        using (var wrappedClient = new HttpClient { BaseAddress = new Uri(baseAddress) })
        {
            var wrapped = await wrappedClient.PostAsJsonAsync("/api/members/register", new RegisterMemberInput("bad", "1", ""));
            Ensure(wrapped.StatusCode == HttpStatusCode.OK && wrapped.Headers.Contains("x-wrap-result"),
                "error wrapped as AjaxResponse envelope by default");
        }

        // 3. 会员注册：非法输入 → ExceptionFilter 包装为 AjaxResponse（success=false）
        var badRegister = await client.PostAsJsonAsync("/api/members/register", new RegisterMemberInput("not-an-email", "123", ""));
        var badBody = await badRegister.Content.ReadFromJsonAsync<JsonElement>();
        Ensure(!badBody.GetProperty("success").GetBoolean() && badBody.GetProperty("error").ValueKind == JsonValueKind.Object,
            "invalid register rejected via validation envelope");

        // 4. 会员注册：合法 → 事件处理器发验证码短信 + 验证邮件
        var register = await client.PostAsJsonAsync("/api/members/register", new RegisterMemberInput("alice@example.com", "+8613800000000", "Alice"));
        Ensure(register.IsSuccessStatusCode, "member registered");
        var member = await register.Content.ReadFromJsonAsync<JsonElement>();
        var memberId = member.GetProperty("id").GetGuid();

        await WaitUntilAsync(() => NotificationLog.Entries.Contains($"member-registered:{memberId}"));
        Ensure(NotificationLog.Entries.Contains($"member-registered:{memberId}"), "registration event handled (sms+email sent)");

        // 4b. 用短信里的验证码完成验证（注册 → 验证 闭环）
        var verificationCode = NotificationLog.Entries
            .FirstOrDefault(e => e.StartsWith($"member-code:{memberId}:"))?.Split(':')[2];
        var verify = await client.PostAsJsonAsync("/api/members/verify", new VerifyMemberInput(memberId, verificationCode ?? string.Empty));
        var verified = await verify.Content.ReadFromJsonAsync<JsonElement>();
        Ensure(verified.GetProperty("isVerified").GetBoolean(), "member verified with sms code");

        // 5. 商品列表：两次调用第二次命中缓存
        var products1 = await client.GetFromJsonAsync<JsonElement>("/api/products");
        Ensure(products1.GetArrayLength() == 2, "product list served");
        var products2 = await client.GetFromJsonAsync<JsonElement>("/api/products");
        Ensure(products2.GetArrayLength() == 2, "product list cached");

        // 6. 权限：匿名改价 → AuthorizationException 信封（unAuthorizedRequest=true）；admin 角色改价 → 成功
        var denied = await client.PutAsJsonAsync($"/api/products/{DataSeeder.KeyboardId}/price", new UpdatePriceInput(499m));
        var deniedRaw = await denied.Content.ReadAsStringAsync();
        var deniedBody = JsonSerializer.Deserialize<JsonElement>(deniedRaw);
        // 当前 ExceptionFilter 的错误转换只保留 UserFriendlyException 的 code，其余 masked；断言信封语义
        Ensure(!deniedBody.GetProperty("success").GetBoolean()
               && deniedBody.GetProperty("error").ValueKind == JsonValueKind.Object,
            $"anonymous price update denied (actual: {deniedRaw})");

        using (var request = new HttpRequestMessage(HttpMethod.Put, $"/api/products/{DataSeeder.KeyboardId}/price"))
        {
            request.Headers.Add("X-Demo-User", "u-1001");
            request.Headers.Add("X-Demo-Roles", "admin");
            request.Content = JsonContent.Create(new UpdatePriceInput(499m));
            var updated = await client.SendAsync(request);
            Ensure(updated.IsSuccessStatusCode, "admin price update granted");
        }

        // 7. 当前用户（Security / ICurrentUser）
        using (var request = new HttpRequestMessage(HttpMethod.Get, "/api/me"))
        {
            request.Headers.Add("X-Demo-User", "u-1001");
            request.Headers.Add("X-Demo-Roles", "admin");
            var me = await (await client.SendAsync(request)).Content.ReadFromJsonAsync<JsonElement>();
            Ensure(me.GetProperty("isAuthenticated").GetBoolean(), "current user authenticated");
        }

        // 8. 下单：验证 + 免邮门槛设置 + 静态代理定价
        var placed = await client.PostAsJsonAsync("/api/orders", new PlaceOrderInput("alice@example.com",
            [new PlaceOrderItemInput(DataSeeder.KeyboardId, 1)]));
        Ensure(placed.IsSuccessStatusCode, "order placed");
        var order = await placed.Content.ReadFromJsonAsync<JsonElement>();
        var orderId = order.GetProperty("id").GetGuid();
        // 499 >= 免邮门槛 99 → 应付 = 总额（免运费）
        Ensure(order.GetProperty("payableAmount").GetDecimal() == order.GetProperty("totalAmount").GetDecimal(), "free shipping applied via settings+proxy");
        Ensure(AuditInterceptor.Invocations.Contains("CalculatePayableAsync"), "interceptor audited proxied call");

        // 9. 确认订单 → 事务提交后事件处理器渲染模板并发确认邮件
        var confirmed = await client.PostAsync($"/api/orders/{orderId}/confirm", content: null);
        Ensure(confirmed.IsSuccessStatusCode, "order confirmed");
        await WaitUntilAsync(() => NotificationLog.Entries.Contains($"order-confirmed:{orderId}"));
        Ensure(NotificationLog.Entries.Contains($"order-confirmed:{orderId}"), "confirmation event handled (templated email sent)");

        // 10. 健康检查
        var health = await client.GetAsync("/health");
        Ensure(health.IsSuccessStatusCode, "health endpoint");

        return failures.Count == 0 ? 0 : 1;
    }

    private static async Task WaitUntilAsync(Func<bool> condition, int timeoutMs = 5000)
    {
        var start = Environment.TickCount64;
        while (!condition() && Environment.TickCount64 - start < timeoutMs)
        {
            await Task.Delay(50);
        }
    }
}
