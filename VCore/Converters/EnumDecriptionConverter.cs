﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace VCore.WPF.Converters
{
  public class EnumDecriptionConverter : BaseConverter
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value != null)
      {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
          typeof(DescriptionAttribute), false);

        if (attributes != null && attributes.Length > 0)
          return attributes[0].Description;
        else return value.ToString();
      }

      return value;

    }
  }
}