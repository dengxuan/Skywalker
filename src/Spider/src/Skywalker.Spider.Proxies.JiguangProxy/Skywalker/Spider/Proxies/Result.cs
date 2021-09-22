using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Skywalker.Spider.Proxies
{
    public class Result
    {
        public bool Success { get; set; }

        public string Msg { get; set; }

        public int Code { get; set; }

        public List<Data>? Data { get; set; }
    }

    public class Data
    {
        public string Ip { get; set; }

        public ushort Port { get; set; }


        [JsonPropertyName("expire_time")]
        public string ExpireTime { get; set; }

    }
}
