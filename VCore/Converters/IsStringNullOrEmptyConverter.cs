using System;
using System.Globalization;

namespace VCore.WPF.Converters
{
  public class IsStringNullOrEmptyConverter : BaseConverter
  {
    public override object Convert(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return string.IsNullOrEmpty(value?.ToString());
    }
  }
}