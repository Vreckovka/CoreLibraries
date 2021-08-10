using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using VCore.Converters;

namespace VCore.WPF.Converters
{
  public class IsNullConverter : BaseConverter
  {
    public override object Convert(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return value == null;
    }
  }
}
