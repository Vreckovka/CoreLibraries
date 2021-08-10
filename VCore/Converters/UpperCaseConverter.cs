using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using VCore.Converters;

namespace VCore.WPF.Converters
{
  public enum ConvertType
  {
    AllUpper,
    Capitalize
  }
  public class UpperCaseConverter : BaseConverter
  {
    public ConvertType ConvertType { get; set; }

    public override object Convert(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      switch (ConvertType)
      {
        case ConvertType.AllUpper:
          return value?.ToString()?.ToUpper();
        case ConvertType.Capitalize:
          {
            var valueString = value?.ToString();

            if (!string.IsNullOrEmpty(valueString))
            {
              var words = valueString.Split(" ");

              List<string> finalWords = new List<string>();

              foreach (var word in words)
              {
                if(!string.IsNullOrEmpty(word))
                {
                  var fistLetter = word[0];
                  var next = word.Substring(1);

                  finalWords.Add(fistLetter.ToString().ToUpper() + next);
                }
               
              }

              valueString = finalWords.Aggregate((x, y) => x + " "+ y);
            }

            return valueString;

          }
          return value?.ToString()?.ToUpper();
        default:
          throw new ArgumentOutOfRangeException();
      }

    }
  }
}
