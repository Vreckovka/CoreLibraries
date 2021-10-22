using System;
using System.Globalization;
using System.Windows;

namespace VCore.WPF.Converters
{
  public class VisibilityConverter : BaseConverter
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool show)
      {
        if (show)
        {
          return Visibility.Visible;
        }
        else
          return Visibility.Hidden;
      }

      return Visibility.Collapsed;
    }
  }
}
