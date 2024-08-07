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
      var value = double.Parse(values[0].ToString()) / double.Parse(values[1].ToString());

      return value * 100;
    }
  }
}