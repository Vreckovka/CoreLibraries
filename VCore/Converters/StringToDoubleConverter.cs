using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using VCore.Converters;

namespace VCore.WPF.Converters
{
  public class StringToDoubleConverter : BaseConverter
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var stringValue = value?.ToString()?.Replace(",", ".");

      double.TryParse(stringValue, out var parsedValue);

      return parsedValue;
    }
  }
}
