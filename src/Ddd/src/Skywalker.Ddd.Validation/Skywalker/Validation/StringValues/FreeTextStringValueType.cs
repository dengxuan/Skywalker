﻿using System;

namespace Skywalker.Validation.StringValues
{
    [Serializable]
    [StringValueType("FREE_TEXT")]
    public class FreeTextStringValueType : StringValueTypeBase
    {
        public FreeTextStringValueType()
        {

        }

        public FreeTextStringValueType(IValueValidator validator)
            : base(validator)
        {
        }
    }
}