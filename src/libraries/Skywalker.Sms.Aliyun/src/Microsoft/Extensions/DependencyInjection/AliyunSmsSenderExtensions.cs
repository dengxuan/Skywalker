// Licensed to the zshiot.com under one or more agreements.
// zshiot.com licenses this file to you under the license.

using Skywalker.Sms.Abstractions;
using Skywalker.Sms.Aliyun;

namespace Microsoft.Extensions.DependencyInjection;

public static class AliyunSmsSenderExtensions
{
    public static IServiceCollection AddAliyunSms(this IServiceCollection service, Action<AbpAliyunSmsOptions> options)
    {
        service.Configure(options);

        service.AddSingleton<ISmsSender, AliyunSmsSender>();

        return service;
    }
}
