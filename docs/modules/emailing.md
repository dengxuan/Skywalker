# 邮件服务模块

本文档详细介绍 Skywalker 框架的邮件服务模块。

> v2.0 起，Emailing 是独立的增强功能家族，结构对标 `Skywalker.Sms.*`：契约层 + 可插拔投递 provider。组件只负责**发送**——邮件内容（主题/正文）由调用方构造好传入；需要模板渲染时，请使用 Template 家族（Razor/Scriban）渲染后再发送，用什么模板引擎、什么模板内容完全由应用决定。

---

## 目录

1. [模块概述](#模块概述)
2. [Skywalker.Emailing.Abstractions](#skywalkeremailingabstractions)
3. [Skywalker.Emailing.Smtp](#skywalkeremailingsmtp)
4. [配合模板引擎使用](#配合模板引擎使用)

---

## 模块概述

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| Skywalker.Emailing.Abstractions | `Skywalker.Emailing.Abstractions` | 契约层：`IEmailSender`、发件人配置、`NullEmailSender` 兜底 |
| Skywalker.Emailing.Smtp | `Skywalker.Emailing.Smtp` | SMTP 直连投递 provider |

未来的三方代发 provider（如 DirectMail、SendGrid）作为并列包加入家族。

### 依赖关系

```
Skywalker.Emailing.Smtp                ← SMTP 投递 provider
└── Skywalker.Emailing.Abstractions    ← 契约层
    └── Skywalker.Extensions.Universal
```

---

## Skywalker.Emailing.Abstractions

### 简介

邮件发送契约层。未注册投递 provider 时默认使用 `NullEmailSender`（仅记录日志，不实际发送），便于开发期先行集成。

### 安装

```bash
dotnet add package Skywalker.Emailing.Abstractions   # 声明依赖只需契约层
dotnet add package Skywalker.Emailing.Smtp           # 实际投递装 provider
```

### 核心类型

#### IEmailSender - 邮件发送器接口

```csharp
namespace Skywalker.Emailing;

public interface IEmailSender
{
    /// <summary>
    /// 发送邮件
    /// </summary>
    Task SendAsync(string to, string subject, string body, bool isBodyHtml = true);

    /// <summary>
    /// 发送邮件（指定发件人）
    /// </summary>
    Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml = true);

    /// <summary>
    /// 发送邮件（MailMessage）
    /// </summary>
    Task SendAsync(MailMessage mail, bool normalize = true);
}
```

#### EmailSenderOptions - 发送通用配置

```csharp
namespace Skywalker.Emailing;

public class EmailSenderOptions
{
    /// <summary>
    /// 默认发件人地址
    /// </summary>
    public string? DefaultFromAddress { get; set; }

    /// <summary>
    /// 默认发件人显示名称
    /// </summary>
    public string? DefaultFromDisplayName { get; set; }
}
```

### 服务注册

```csharp
// 仅契约层：NullEmailSender 兜底（记录日志，不发送）
services.AddEmailing(options =>
{
    options.DefaultFromAddress = "noreply@example.com";
    options.DefaultFromDisplayName = "My Application";
});
```

### 使用示例

```csharp
public class NotificationService
{
    private readonly IEmailSender _emailSender;

    public NotificationService(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task SendWelcomeEmailAsync(string email, string userName)
    {
        // 内容由调用方构造
        var body = $@"
            <h1>欢迎 {userName}!</h1>
            <p>感谢您注册我们的服务。</p>
        ";

        await _emailSender.SendAsync(email, "欢迎加入", body);
    }
}
```

---

## Skywalker.Emailing.Smtp

### 简介

SMTP 直连投递 provider。注册后以 `SmtpEmailSender` 替代 `NullEmailSender` 作为 `IEmailSender` 实现。

### 核心类型

#### ISmtpEmailSender - SMTP 邮件发送器接口

```csharp
namespace Skywalker.Emailing.Smtp;

public interface ISmtpEmailSender : IEmailSender
{
    /// <summary>
    /// 创建并配置 SmtpClient
    /// </summary>
    Task<SmtpClient> BuildClientAsync();
}
```

#### SmtpEmailSenderConfiguration - SMTP 配置

继承 `EmailSenderOptions`（发件人默认地址/显示名），追加 SMTP 连接配置：

```csharp
namespace Skywalker.Emailing.Smtp;

public class SmtpEmailSenderConfiguration : EmailSenderOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Domain { get; set; }
    public bool EnableSsl { get; set; }
    public bool UseDefaultCredentials { get; set; }
}
```

### 服务注册

```csharp
services.AddSmtpEmailing(options =>
{
    options.Host = "smtp.example.com";
    options.Port = 587;
    options.UserName = "your-username";
    options.Password = "your-password";
    options.EnableSsl = true;
    options.DefaultFromAddress = "noreply@example.com";
    options.DefaultFromDisplayName = "My Application";
});
```

---

## 配合模板引擎使用

邮件内容如何生成不是 Emailing 组件的职责。需要模板化邮件时，用 Template 家族（或任何你选择的渲染方案）渲染出主题/正文，再交给 `IEmailSender` 发送：

```csharp
public class WelcomeMailer(ITemplateRenderer renderer, IEmailSender emailSender)
{
    public async Task SendAsync(string email, object model)
    {
        // 模板由应用自己定义、注册（Skywalker.Template.Razor / Scriban 均可）
        var subject = await renderer.RenderAsync("Welcome.Subject", model);
        var body = await renderer.RenderAsync("Welcome.Body", model);

        await emailSender.SendAsync(email, subject.Trim(), body);
    }
}
```
