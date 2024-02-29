﻿using System;
using System.ComponentModel;
using System.Reflection;

namespace Presentation.Enums
{
    public static class EnumExtension
    {
        public static string ToDescription<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }
    }
}
