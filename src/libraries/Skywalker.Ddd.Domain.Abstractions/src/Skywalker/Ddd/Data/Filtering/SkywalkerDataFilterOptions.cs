using System;
using System.Collections.Generic;

namespace Skywalker.Ddd.Data.Filtering
{
    public class SkywalkerDataFilterOptions
    {
        public Dictionary<Type, DataFilterState> DefaultStates { get; }

        public SkywalkerDataFilterOptions()
        {
            DefaultStates = new Dictionary<Type, DataFilterState>();
        }
    }
}
