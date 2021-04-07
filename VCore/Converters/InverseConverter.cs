using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using VCore.Converters;

namespace VCore.WPF.Converters
{
  public class InverseConverter : BaseConverter
  {
    public override object Convert(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      if (value is bool boolValue)
      {
        return !boolValue;
      }

      return value;
    }
  }
}
