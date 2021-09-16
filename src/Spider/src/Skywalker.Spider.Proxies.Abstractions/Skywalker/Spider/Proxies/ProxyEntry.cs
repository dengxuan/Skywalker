using System;

namespace Skywalker.Spider.Proxies;

public class ProxyEntry
{
    /// <summary>
    /// 代理地址
    /// </summary>
    public Uri Uri { get; }

    /// <summary>
    /// 到期时间
    /// </summary>
    public TimeSpan Limited { get; set; }

    /// <summary>
    /// 使用此代理下载数据的失败次数
    /// </summary>
    internal int FailureCount { get; set; }

    /// <summary>
    /// 使用此代理下载数据的成功次数
    /// </summary>
    internal int SuccessCount { get; set; }

    public ProxyEntry(Uri uri, TimeSpan? limited = null)
    {
        Uri = uri;
        Limited = limited ?? TimeSpan.MaxValue;
    }
}
