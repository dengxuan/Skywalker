namespace Skywalker.Emailing.Smtp;

/// <summary>
/// SMTP 投递配置。发件人默认地址等通用配置继承自 <see cref="EmailSenderOptions"/>。
/// </summary>
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
