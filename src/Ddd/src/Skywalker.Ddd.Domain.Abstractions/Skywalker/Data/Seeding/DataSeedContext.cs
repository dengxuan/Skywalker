using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Data.Seeding
{
    public class DataSeedContext
    {
        /// <summary>
        /// Gets/sets a key-value on the <see cref="Properties"/>.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <returns>
        /// Returns the value in the <see cref="Properties"/> dictionary by given <see cref="name"/>.
        /// Returns null if given <see cref="name"/> is not present in the <see cref="Properties"/> dictionary.
        /// </returns>
        [MaybeNull]
        public object this[string name]
        {
            get => Properties.GetOrDefault(name);
            set => Properties[name] = value;
        }

        /// <summary>
        /// Can be used to get/set custom properties.
        /// </summary>
        [NotNull]
        public Dictionary<string, object> Properties { get; }

        public DataSeedContext()
        {
            Properties = new Dictionary<string, object>();
        }

        /// <summary>
        /// Sets a property in the <see cref="Properties"/> dictionary.
        /// This is a shortcut for nested calls on this object.
        /// </summary>
        public virtual DataSeedContext WithProperty(string key, object value)
        {
            Properties[key] = value;
            return this;
        }
    }
}