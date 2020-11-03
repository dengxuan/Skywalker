﻿using Microsoft.Extensions.Localization;
using Skywalker.Localization.Abstractions;

namespace Skywalker.Localization
{
    public class FixedLocalizableString : ILocalizableString
    {
        public string Value { get; }

        public FixedLocalizableString(string value)
        {
            Value = value;
        }

        public LocalizedString Localize(IStringLocalizerFactory stringLocalizerFactory)
        {
            return new LocalizedString(Value, Value);
        }
    }
}