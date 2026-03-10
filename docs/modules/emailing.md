# 邮件服务模块

本文档详细介绍 Skywalker 框架的邮件服务模块。

---

## 目录

1. [模块概述](#模块概述)
2. [Skywalker.Extensions.Emailing](#skywalkerextensionsemailing)
3. [Skywalker.Extensions.Emailing.Template](#skywalkerextensionsemailingtemplate)

---

## 模块概述

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| Skywalker.Extensions.Emailing | `Skywalker.Extensions.Emailing` | 邮件发送服务 |
| Skywalker.Extensions.Emailing.Template | `Skywalker.Extensions.Emailing.Template` | 模板邮件支持 |

### 依赖关系

```
Skywalker.Extensions.Emailing.Template
├── Skywalker.Extensions.Emailing
├── Skywalker.Template.Abstractions
└── Skywalker.Extensions.VirtualFileSystem
```

---

## Skywalker.Extensions.Emailing

### 简介

邮件发送服务，支持 SMTP 协议。

### 安装

```bash
dotnet add package Skywalker.Extensions.Emailing
```

### 核心类型

#### IEmailSender - 邮件发送器接口

```csharp
namespace Skywalker.Extensions.Emailing;

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

#### ISmtpEmailSender - SMTP 邮件发送器接口

```csharp
namespace Skywalker.Extensions.Emailing.Smtp;

public interface ISmtpEmailSender : IEmailSender
{
    /// <summary>
    /// 创建并配置 SmtpClient
    /// </summary>
    Task<SmtpClient> BuildClientAsync();
}
```

#### SmtpEmailSenderConfiguration - SMTP 配置

```csharp
namespace Skywalker.Extensions.Emailing;

public class SmtpEmailSenderConfiguration
{
    /// <summary>
    /// 默认发件人地址
    /// </summary>
    public string? DefaultFromAddress { get; set; }
    
    /// <summary>
    /// 默认发件人显示名称
    /// </summary>
    public string? DefaultFromDisplayName { get; set; }
    
    /// <summary>
    /// SMTP 服务器地址
    /// </summary>
    public string? Host { get; set; }
    
    /// <summary>
    /// SMTP 端口
    /// </summary>
    public int Port { get; set; }
    
    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; set; }
    
    /// <summary>
    /// 密码
    /// </summary>
    public string? Password { get; set; }
    
    /// <summary>
    /// 域名
    /// </summary>
    public string? Domain { get; set; }
    
    /// <summary>
    /// 是否启用 SSL
    /// </summary>
    public bool EnableSsl { get; set; }
    
    /// <summary>
    /// 是否使用默认凭据
    /// </summary>
    public bool UseDefaultCredentials { get; set; }
}
```

### 服务注册

```csharp
services.AddEmailing(options =>
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
        var body = $@"
            <h1>欢迎 {userName}!</h1>
            <p>感谢您注册我们的服务。</p>
        ";
        
        await _emailSender.SendAsync(email, "欢迎加入", body);
    }
}
```

---

## Skywalker.Extensions.Emailing.Template

### 简介

模板邮件支持，结合模板引擎发送格式化邮件。

### 安装

```bash
dotnet add package Skywalker.Extensions.Emailing.Template
```

### 核心类型

#### ITemplateEmailSender - 模板邮件发送器接口

```csharp
namespace Skywalker.Extensions.Emailing.Template;

public interface ITemplateEmailSender
{
    /// <summary>
    /// 使用模板发送邮件
    /// </summary>
    Task SendAsync(string to, string templateName, object? model = null, string? cultureName = null);
    
    /// <summary>
    /// 使用模板发送邮件（指定发件人）
    /// </summary>
    Task SendAsync(string from, string to, string templateName, object? model = null, string? cultureName = null);
    
    /// <summary>
    /// 使用模板发送邮件（多收件人）
    /// </summary>
    Task SendAsync(IEnumerable<string> to, string templateName, object? model = null, string? cultureName = null);
}
```

#### EmailTemplateOptions - 模板选项

```csharp
namespace Skywalker.Extensions.Emailing.Template;

public class EmailTemplateOptions
{
    /// <summary>
    /// 主题模板后缀，默认 ".Subject"
    /// </summary>
    public string SubjectTemplateSuffix { get; set; } = ".Subject";
    
    /// <summary>
    /// 正文模板后缀，默认 ".Body"
    /// </summary>
    public string BodyTemplateSuffix { get; set; } = ".Body";
    
    /// <summary>
    /// 默认布局模板名称
    /// </summary>
    public string? DefaultLayoutTemplate { get; set; }
    
    /// <summary>
    /// 模板基础路径，默认 "EmailTemplates"
    /// </summary>
    public string TemplateBasePath { get; set; } = "EmailTemplates";
}
```

