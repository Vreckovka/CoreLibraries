using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace VCore.WPF.Converters
{
  public class ImageSourceConverter : BaseConverter
  {
    public override object Convert(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return value?.ToString();
    }
  }
}
