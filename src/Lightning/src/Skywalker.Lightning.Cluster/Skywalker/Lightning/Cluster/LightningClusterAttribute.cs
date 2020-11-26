using System;

namespace Skywalker.Lightning.Cluster
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class LightningClusterAttribute : Attribute
    {
        public string Id { get; set; }

        public LightningClusterAttribute(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }
    }
}
