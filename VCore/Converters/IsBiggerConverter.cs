using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using VCore.Converters;

namespace VPlayer.Library
{
  public class IsBiggerConverter : MarkupExtension, IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length == 2 && values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue)
      {
        if (System.Convert.ToDouble(values[0]) > System.Convert.ToDouble(values[1]))
          return true;
      }
      else if (values[0] != DependencyProperty.UnsetValue && values.Length == 1)
      {
        if (System.Convert.ToDouble(values[0]) > 0)
          return true;
      }

      return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      return null;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }
  }

  public class ExtractMilisecondFromTimeSpanConverter : MarkupExtension, IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length == 2 && 
          TimeSpan.TryParse(values[0]?.ToString(), out var time) &&
          double.TryParse(values[1]?.ToString(), out var miliseconds))
      {
        var asd = TimeSpan.FromMilliseconds(time.TotalMilliseconds + miliseconds).ToString(@"hh\:mm\:ss");

        return asd;
      }
     
      return "Ahoj";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      return null;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }
  }

  public class IsGreaterConverter : BaseConverter
  {
    public override object Convert(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      if (double.TryParse(value?.ToString(), out var doubleValue) &&
          double.TryParse(parameter?.ToString(), out var doubleParameter))
      {
        return doubleValue > doubleParameter;
      }

      return value;
    }
  }
}