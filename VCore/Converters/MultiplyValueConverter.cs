using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VCore.WPF.Converters;

namespace VPlayer.Home.Converters
{
  public class MultiplyValueConverter : BaseConverter
  {
    public override object Convert(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      if (double.TryParse(value?.ToString(), out var count) && double.TryParse(parameter?.ToString(), out var parameterDouble))
      {
        return count * parameterDouble;
      }

      return 0;
    }
  }

  //Add string format '{}{0:N2}' to multibiding if error with string 
  public class BasicMathOperationConverter : BaseMultiValueConverter
  {
    public override object Convert(
      object[] values,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      var list = new List<double>();
      double result = 0;

      foreach (var value in values)
      {
        double valueNumber = 0;

        if (double.TryParse(value?.ToString(), out valueNumber))
        {
          list.Add(valueNumber);
        }
        else
        {
          return 0;
        }
      }

      if (list.Any())
      {
        result = list[0];

        list.RemoveAt(0);

        if (parameter != null && list.Any())
        {
          switch (parameter.ToString())
          {
            case "*":
              foreach (var value in list)
              {
                result *= value;
              }
              break;
            case "+":
              foreach (var value in list)
              {
                result += value;
              }
              break;
            case "-":
              foreach (var value in list)
              {
                result -= value;
              }
              break;
            case "/":
              foreach (var value in list)
              {
                result /= value;
              }
              break;
          }
        }
      }

      return result;
    }
  }
}
