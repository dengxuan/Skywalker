namespace Skywalker.Spider.Proxies
{
    public class ProxyOptions
    {
        public string? ConnectionString { get; set;  }

        public string ProxyTestUrl { get; set; } = "http://www.bing.com";

        public int RefreshInterval { get; set; } = 10000;

        public int IgnoreCount { get; set; }

        public int RedetectCount { get; set; }
    }
}
