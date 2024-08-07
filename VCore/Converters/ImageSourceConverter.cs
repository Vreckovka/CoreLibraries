using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
