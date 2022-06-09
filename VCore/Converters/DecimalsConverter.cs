using System;
using System.Globalization;

namespace VCore.WPF.Converters
{
  public class DecimalsConverter : BaseMultiValueConverter
  {
    public override object Convert(
      object[] values,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return double.Parse(values[0].ToString()).ToString($"N{values[1]}") + parameter;
    }
  }
}