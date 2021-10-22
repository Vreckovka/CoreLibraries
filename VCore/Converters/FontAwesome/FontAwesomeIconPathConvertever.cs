using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace VCore.WPF.Converters.FontAwesome
{
  public class FontAwesomeIconPathConverter : BaseMultiValueConverter
  {
    public override object Convert(
      object[] values,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      if (values.Length >= 2)
      {
        var iconName = values[0].ToString();
        var iconType = values[1].ToString();

        var iconPath = $"{parameter}\\{iconType}\\{iconName}.svg";

        var file = File.ReadAllText(iconPath);
        var match = Regex.Match(file, "<path d=\"(.*)\"");
        var data = match.Groups[1].Value;

        return Geometry.Parse(data);
      }

      return values;
    }
  }
}
