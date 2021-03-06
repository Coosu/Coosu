﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Coosu.Beatmap.Configurable
{
    public class SectionConverterAttribute : Attribute
    {
        private readonly Type _converterType;
        private readonly object[] _param;

        public SectionConverterAttribute(Type converterType, params object[] param)
        {
            if (!converterType.IsSubclassOf(typeof(ValueConverter)))
                throw new Exception($"Type {converterType} isn\'t a converter.");
            _converterType = converterType;
            _param = param;
        }

        public SectionConverterAttribute(Type converterType) : this(converterType, null)
        {

        }

        public ValueConverter GetConverter()
        {
            if (_param == null)
                return (ValueConverter)Activator.CreateInstance(_converterType);
            return (ValueConverter)Activator.CreateInstance(_converterType, _param);
        }
    }
}
