namespace Skywalker.Emailing;

/// <summary>
/// 邮件发送通用配置：默认发件人地址与显示名。
/// 具体投递 provider（如 SMTP）的连接配置继承本类型扩展。
/// </summary>
public class EmailSenderOptions
{
    public string? DefaultFromAddress { get; set; }
    public string? DefaultFromDisplayName { get; set; }
}
