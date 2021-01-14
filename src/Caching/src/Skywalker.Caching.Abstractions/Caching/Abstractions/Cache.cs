using System;

namespace Skywalker.Caching.Abstractions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CacheAttribute : Attribute
    {
        public string Key { get; }

        /// <summary>
        /// The absolute expire time of seconds
        /// </summary>
        public int Expiry { get; set; }

        public CacheAttribute(string key)
        {
            Key = key;
        }
    }
}
