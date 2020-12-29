using System;
using System.Collections.Generic;
using System.Text;

namespace Skywalker.Aspects
{
    public class AspectsAttribute : Attribute
    {
        public bool Disable { get; set; } = false;
    }
}
