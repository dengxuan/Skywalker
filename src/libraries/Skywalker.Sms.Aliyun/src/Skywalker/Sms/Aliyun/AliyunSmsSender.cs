using Microsoft.Extensions.Options;
using Skywalker.Extensions.DependencyInjection;
using Skywalker.Sms.Abstractions;
using AliyunClient = AlibabaCloud.SDK.Dysmsapi20170525.Client;
using AliyunConfig = AlibabaCloud.OpenApiClient.Models.Config;
using AliyunSendSmsRequest = AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest;

namespace Skywalker.Sms.Aliyun;

public class AliyunSmsSender : ISmsSender, ISingletonDependency
{
    protected AbpAliyunSmsOptions Options { get; }

    public AliyunSmsSender(IOptionsMonitor<AbpAliyunSmsOptions> options)
    {
        Options = options.CurrentValue;
    }

    public async Task SendAsync(SmsMessage smsMessage)
    {
        var client = CreateClient();

        await client.SendSmsAsync(new AliyunSendSmsRequest
        {
            PhoneNumbers = smsMessage.PhoneNumber,
            SignName = smsMessage.Properties.GetOrDefault("SignName") as string,
            TemplateCode = smsMessage.Properties.GetOrDefault("TemplateCode") as string,
            TemplateParam = smsMessage.Text
        });
    }

    protected virtual AliyunClient CreateClient()
    {
        return new(new AliyunConfig
        {
            AccessKeyId = Options.AccessKeyId,
            AccessKeySecret = Options.AccessKeySecret,
            RegionId = Options.RegionId,
            Endpoint = "dysmsapi.aliyuncs.com",
        });
    }
}
