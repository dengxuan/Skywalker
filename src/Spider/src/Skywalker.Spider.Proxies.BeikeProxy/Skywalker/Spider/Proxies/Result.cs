using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Skywalker.Spider.Proxies
{
    public class Result
    {
        public string Status { get; set; }

        public string Msg { get; set; }

        public int Code { get; set; }

        public List<Data>? Data { get; set; }
    }

    public class Data
    {
        public string Protocol { get; set; }

        [JsonPropertyName("expire_time")]
        public string ExpireTime { get; set; }

        public string Ip { get; set; }

        public ushort Port { get; set; }
    }
}
