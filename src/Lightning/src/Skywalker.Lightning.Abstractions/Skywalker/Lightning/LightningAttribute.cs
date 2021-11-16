using System;

namespace Skywalker.Lightning
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LightningAttribute : Attribute
    {
        public bool Disabled { get; set; }
    }
}
