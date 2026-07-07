using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Skywalker.DependencyInjection;
using Skywalker.Emailing;
using Skywalker.EventBus.Abstractions;
using Skywalker.Sample.Website.Domain;
using Skywalker.Sms;
using Skywalker.Sms.Abstractions;
using Skywalker.Template.Abstractions;

namespace Skywalker.Sample.Website.Application;

/// <summary>
/// smoke 验证用：记录事件处理器实际完成的动作。
/// </summary>
public static class NotificationLog
{
    public static ConcurrentQueue<string> Entries { get; } = new();
}

/// <summary>
/// 会员注册事件处理器：事务提交后发送验证码短信（ISmsSender）
/// 与验证邮件（模板由站点自己定义，Scriban 渲染后经 IEmailSender 发送）。
/// </summary>
[EventHandler]
public sealed class MemberRegisteredEventHandler(
    ISmsSender smsSender,
    IEmailSender emailSender,
    ITemplateRenderer templateRenderer,
    ILogger<MemberRegisteredEventHandler> logger) : IEventHandler<MemberRegisteredEvent>
{
    public async Task HandleEventAsync(MemberRegisteredEvent eventData)
    {
        await smsSender.SendAsync(new SmsMessage(eventData.Phone, $"[Skywalker] 验证码：{eventData.VerificationCode}"));

        var body = await templateRenderer.RenderAsync(
            WebsiteTemplates.VerificationEmail,
            new { name = eventData.DisplayName, code = eventData.VerificationCode });
        await emailSender.SendAsync(eventData.Email, "请验证你的邮箱", body);

        // 记录发出的验证码（真实场景在短信/邮件里；smoke 用它完成 注册→验证 闭环）
        NotificationLog.Entries.Enqueue($"member-code:{eventData.MemberId}:{eventData.VerificationCode}");
        NotificationLog.Entries.Enqueue($"member-registered:{eventData.MemberId}");
        logger.LogInformation("Verification sms/email sent to {Email}", eventData.Email);
    }
}

/// <summary>
/// 订单确认事件处理器：渲染确认邮件模板并发送。
/// </summary>
[EventHandler]
public sealed class OrderConfirmedEventHandler(
    IEmailSender emailSender,
    ITemplateRenderer templateRenderer,
    ILogger<OrderConfirmedEventHandler> logger) : IEventHandler<OrderConfirmedEvent>
{
    public async Task HandleEventAsync(OrderConfirmedEvent eventData)
    {
        var body = await templateRenderer.RenderAsync(
            WebsiteTemplates.OrderConfirmedEmail,
            new { order_no = eventData.OrderNo, amount = eventData.TotalAmount });
        await emailSender.SendAsync(eventData.CustomerEmail, $"订单 {eventData.OrderNo} 已确认", body);

        NotificationLog.Entries.Enqueue($"order-confirmed:{eventData.OrderId}");
        logger.LogInformation("Order confirmation email sent for {OrderNo}", eventData.OrderNo);
    }
}
