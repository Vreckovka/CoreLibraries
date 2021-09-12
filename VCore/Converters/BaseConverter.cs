using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace VCore.Converters
{
  public abstract class BaseConverter : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public abstract class BaseMultiValueConverter : MarkupExtension, IMultiValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public abstract object Convert(
      object[] values,
      Type targetType,
      object parameter,
      CultureInfo culture);

    public virtual object[] ConvertBack(
      object value,
      Type[] targetTypes,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
