using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using VCore.Converters;
using Color = System.Windows.Media.Color;

namespace VCore.WPF.Converters
{
  public class StringToColorConverter : BaseConverter
  {
    public override object Convert(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return (Color)System.Windows.Media.ColorConverter.ConvertFromString(value.ToString());
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
     if(value is Color color)
     {
       return color.ToString();
     }

     return null;
    }
  }
}
