using System;
using System.Globalization;
using System.Linq;

namespace VCore.WPF.Converters
{
  public enum TimeType
  {
    Second,
    Minutes,
    Hours
  }
  public class TimeConverter : BaseConverter
  {
    #region TimeType

    public TimeType TimeType { get; set; }

    #endregion

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value != null)
      {
        if (value is TimeSpan)
        {
          return null;
        }
        else
        {
          var time = System.Convert.ToDouble(value);

          switch (TimeType)
          {
            case TimeType.Second:
              return TimeSpan.FromSeconds(time);
            case TimeType.Minutes:
              return TimeSpan.FromMinutes(time);
            case TimeType.Hours:
              return TimeSpan.FromHours(time);
            default:
              throw new ArgumentOutOfRangeException();
          }
        }
      }
      else
        return null;
    }
  }

  public class CountCharactersAndAddValue : BaseConverter
  {
    public double CharacterWidth { get; set; }
    public override object Convert(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      var count = value?.ToString()?.Count();

      if (count != null && double.TryParse(parameter?.ToString(), out var doubleParameter))
      {
        return doubleParameter + (count * CharacterWidth);
      }

      return value;
    }
  }
}
