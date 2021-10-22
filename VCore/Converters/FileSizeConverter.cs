using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace VCore.WPF.Converters
{
  public class FileSizeConverter : BaseConverter
  {
    public override object Convert(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      if(long.TryParse(value.ToString(), out var size))
      {
        double finalSize = 0;
        string sizeUnit = "B";

        if (size < 999)
        {
          finalSize = size;
          sizeUnit = "B";
        }
        else if (size > 999 && size < 999999)
        {
          finalSize = size / 1000.0;
          sizeUnit = "kB";
        }
        else if (size > 999999 && size < 999999999)
        {
          finalSize = size / 1000000.0;
          sizeUnit = "MB";
        }
        else if (size > 999999999)
        {
          finalSize = size / 1000000000.0;
          sizeUnit = "GB";
        }

        if(parameter != null)
        {
          return finalSize.ToString(parameter.ToString()) + " " + sizeUnit;
        }
        else
        {
          return finalSize + " " + sizeUnit;
        }
      }


      return value;
    }
  }
}
