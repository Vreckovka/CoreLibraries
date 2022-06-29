using System;
using System.Globalization;
using System.Windows;

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
      if (values.Length > 1 && values[0] != null && values[1] != null && values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue)
        return double.Parse(values[0].ToString()).ToString($"N{values[1]}") + parameter;

      return null;
    }
  }
}