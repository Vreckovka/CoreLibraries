using System;
using System.Globalization;

namespace VCore.WPF.Converters
{
  public class IsTypeConverter : BaseConverter
  {
    public override object Convert(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      if (value != null && parameter != null)
      {
        var type = value?.GetType();
        Type paramType = null;

        if (parameter is Type type1)
        {
          paramType = type1;
        }
        else
        {
          paramType = parameter?.GetType();
        }

        if (type == paramType || paramType.IsAssignableFrom(type) || paramType.IsSubclassOf(type))
        {
          return true;
        }
      }

      return false;
    }
  }
}