﻿namespace Skywalker.Ddd.Tracing
{
    public class SkywalkerCorrelationIdOptions
    {
        public string HttpHeaderName { get; set; } = "X-Correlation-Id";

        public bool SetResponseHeader { get; set; } = true;
    }
}
