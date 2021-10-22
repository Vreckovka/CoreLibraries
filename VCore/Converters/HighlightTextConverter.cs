using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace VCore.WPF.Converters
{
  public class HighlightTextConverter : BaseConverter
  {
    public string StartHighlight = "|~S~|";
    public string EndHighlight = "|~E~|";
    public Style RunStyle { get; set; } 
    public SolidColorBrush HighlightBackround { get; set; } = Brushes.Yellow;
    public SolidColorBrush HighlightForeground { get; set; } = Brushes.White;

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string input = value as string;
      if (input != null)
      {
        var textBlock = new TextBlock();
        textBlock.TextWrapping = TextWrapping.Wrap;
        string escapedXml = SecurityElement.Escape(input);

        while (escapedXml.IndexOf(StartHighlight) != -1)
        {
         
          textBlock.Inlines.Add(new Run(escapedXml.Substring(0, escapedXml.IndexOf(StartHighlight)))
          {
            Style = RunStyle
          });

         
          textBlock.Inlines.Add(new Run(escapedXml.Substring(escapedXml.IndexOf(StartHighlight) + 5, escapedXml.IndexOf(EndHighlight) - (escapedXml.IndexOf(StartHighlight) + 5)))
          {
            FontWeight = FontWeights.Bold,
            Background = HighlightBackround,
            Foreground = HighlightForeground,
            Style = RunStyle
          });
      
          escapedXml = escapedXml.Substring(escapedXml.IndexOf(EndHighlight) + 5);
        }

        if (escapedXml.Length > 0)
        {
          textBlock.Inlines.Add(new Run(escapedXml)
          {
            Style = RunStyle
          });
        }

        return textBlock;
      }

      return null;
    }
  }
}
