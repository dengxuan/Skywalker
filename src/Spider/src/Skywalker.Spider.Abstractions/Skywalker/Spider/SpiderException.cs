using System;

namespace Skywalker.Spider
{
    public class SpiderException : Exception
    {
        public SpiderException(string msg) : base(msg)
        {
        }
    }
}
