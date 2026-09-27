using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace VCore.WPF.Converters
{
  public class BooleanAndConverter : BaseMultiValueConverter
  {
    public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      return values.All(x => x is bool value && value);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }

  public class DurationConverter : BaseConverter
  {
    public string StringNumberFormat { get; set; } = @"{0:00}";

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var parameterStr = parameter?.ToString();

      if (value is TimeSpan timeSpan)
      {
        if (parameterStr == "days" || parameterStr == null)
        {
          return timeSpan.ToString("dd\\:hh\\:mm\\:ss");
        }
        else if (parameterStr == "hours")
        {
          var hours = timeSpan.TotalHours;
          var onlyHours = Math.Truncate(hours);

          var minutes = (hours - onlyHours) * 60;
          var onlyMinutes = Math.Truncate(minutes);

          var seconds = Math.Round((minutes - onlyMinutes) * 60);

          if (seconds == 60)
          {
            seconds = 0;
            onlyMinutes++;
          }

          return $"{string.Format(StringNumberFormat, onlyHours)}:{string.Format(StringNumberFormat, onlyMinutes)}:{string.Format(StringNumberFormat, seconds)}";
        }
        else if (parameterStr == "minutes")
        {
          var minutes = timeSpan.TotalMinutes;
          var onlyMinutes = Math.Truncate(minutes);

          var seconds = Math.Round((minutes - onlyMinutes) * 60);

          if (seconds == 60)
          {
            seconds = 0;
            onlyMinutes++;
          }

          return $"{string.Format(StringNumberFormat, onlyMinutes)}:{string.Format(StringNumberFormat, seconds)}";
        }
        else if (parameterStr == "seconds")
        {
          return timeSpan.TotalSeconds.ToString(StringNumberFormat);
        }
      }

      return value;
    }
  }
}
