using System;
using System.Globalization;

namespace VCore.WPF.Converters
{
  public class PercentageConverter : BaseMultiValueConverter
  {
    public override object Convert(
      object[] values,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      if(double.TryParse(values[0].ToString(), out var value1) && 
        double.TryParse(values[1].ToString(), out var value2))
        {
        var value = value1 / value2;

        return value * 100;
      }

      return null;
    }
  }
}