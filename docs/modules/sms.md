# 短信服务模块

本文档详细介绍 Skywalker 框架的短信服务模块。

---

## 目录

1. [模块概述](#模块概述)
2. [Skywalker.Sms.Abstractions](#skywalkersmsabstractions)
3. [Skywalker.Sms.Aliyun](#skywalkersmsaliyun)

---

## 模块概述

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| Skywalker.Sms.Abstractions | `Skywalker.Sms.Abstractions` | 短信抽象接口 |
| Skywalker.Sms.Aliyun | `Skywalker.Sms.Aliyun` | 阿里云短信实现 |

### 依赖关系

```
Skywalker.Sms.Aliyun
├── Skywalker.Sms.Abstractions
├── Skywalker.Extensions.Universal
└── AlibabaCloud.SDK.Dysmsapi20170525 (3.0.0)
```

---

## Skywalker.Sms.Abstractions

### 简介

短信抽象模块，定义短信发送的核心接口。

### 安装

```bash
dotnet add package Skywalker.Sms.Abstractions
```

### 核心类型

#### ISmsSender - 短信发送器接口

```csharp
namespace Skywalker.Sms.Abstractions;

public interface ISmsSender
{
    /// <summary>
    /// 发送短信
    /// </summary>
    Task SendAsync(SmsMessage smsMessage);
}
```

#### SmsMessage - 短信消息

```csharp
namespace Skywalker.Sms;

public class SmsMessage
{
    /// <summary>
    /// 手机号码
    /// </summary>
    public string PhoneNumber { get; }
    
    /// <summary>
    /// 短信内容
    /// </summary>
    public string Text { get; }
    
    /// <summary>
    /// 扩展属性（如模板ID、签名等）
    /// </summary>
    public IDictionary<string, object> Properties { get; }
    
    public SmsMessage(string phoneNumber, string text);
}
```

#### NullSmsSender - 空实现

```csharp
namespace Skywalker.Sms;

/// <summary>
/// 空实现，仅记录日志不实际发送
/// </summary>
public class NullSmsSender : ISmsSender
{
    public Task SendAsync(SmsMessage smsMessage);
}
```

### 扩展方法

```csharp
namespace Skywalker.Sms;

public static class SmsSenderExtensions
{
    /// <summary>
    /// 简化的发送方法
    /// </summary>
    public static Task SendAsync(this ISmsSender smsSender, string phoneNumber, string text);
}
```

### 使用示例

```csharp
public class VerificationService
{
    private readonly ISmsSender _smsSender;
    
    public VerificationService(ISmsSender smsSender)
    {
        _smsSender = smsSender;
    }
    
    public async Task SendVerificationCodeAsync(string phoneNumber, string code)
    {
        await _smsSender.SendAsync(phoneNumber, $"您的验证码是：{code}");
    }
}
```

---

## Skywalker.Sms.Aliyun

### 简介

阿里云短信服务实现。

### 安装

```bash
dotnet add package Skywalker.Sms.Aliyun
```

### 核心类型

#### AliyunSmsSender

```csharp
namespace Skywalker.Sms.Aliyun;

public class AliyunSmsSender : ISmsSender
{
    public AliyunSmsSender(IOptionsMonitor<SkywalkerAliyunSmsOptions> options);
    
    public async Task SendAsync(SmsMessage smsMessage);
}
```

#### SkywalkerAliyunSmsOptions - 配置选项

```csharp
namespace Skywalker.Sms.Aliyun;

public class SkywalkerAliyunSmsOptions
{
    /// <summary>
    /// 阿里云 AccessKey Secret
    /// </summary>
    public string? AccessKeySecret { get; set; }
    
    /// <summary>
    /// 阿里云 AccessKey ID
    /// </summary>
    public string? AccessKeyId { get; set; }
    
    /// <summary>
    /// 区域 ID
    /// </summary>
    public string? RegionId { get; set; }
}
```

### 服务注册

```csharp
services.AddAliyunSms(options =>
{
    options.AccessKeyId = "your-access-key-id";
    options.AccessKeySecret = "your-access-key-secret";
    options.RegionId = "cn-hangzhou";
});
```

### 使用示例

```csharp
public class AliyunSmsService
{
    private readonly ISmsSender _smsSender;
    
    public AliyunSmsService(ISmsSender smsSender)
    {
        _smsSender = smsSender;
    }
    
    public async Task SendVerificationCodeAsync(string phoneNumber, string code)
    {
        var message = new SmsMessage(phoneNumber, JsonSerializer.Serialize(new { code }));
        
        // 设置阿里云短信特有属性
        message.Properties["SignName"] = "您的签名";
        message.Properties["TemplateCode"] = "SMS_123456789";
        
        await _smsSender.SendAsync(message);
    }
}
```

### 配置文件示例

```json
{
  "AliyunSms": {
    "AccessKeyId": "your-access-key-id",
    "AccessKeySecret": "your-access-key-secret",
    "RegionId": "cn-hangzhou"
  }
}
```

```csharp
services.AddAliyunSms(options =>
{
    configuration.GetSection("AliyunSms").Bind(options);
});
```

