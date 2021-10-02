using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace VCore.WPF.Converters
{
  public class CacheImageParameters
  {
    public int? DecodeHeight { get; set; }
    public int? DecodeWidth { get; set; }
  }

  public class CacheImageConverter : MarkupExtension, IValueConverter
  {
    public int? DecodeWidth { get; set; }
    public int? DecodeHeight { get; set; }

    public static Dictionary<string, List<CacheImageConverter>> AllConverters = new Dictionary<string, List<CacheImageConverter>>();
    string lastLoadedImagePath;
    BitmapImage lastLoadedImage;

    public CacheImageConverter()
    {
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    protected BitmapImage GetBitmapImage(object value, object parameter)
    {
      if (value is BitmapImage bitmapImage1)
      {
        return bitmapImage1;
      }

      string path = "";

      if (value != DependencyProperty.UnsetValue && value != null)
      {
        path = value.ToString()?.Replace("file:///", "");
      }

      if (lastLoadedImagePath == path)
      {
        return lastLoadedImage;
      }

      var bitmapImage = new BitmapImage();

      if (path != null)
      {
        bitmapImage.BeginInit();

        if(File.Exists(path))
        {
          bitmapImage.StreamSource = new FileStream(path, FileMode.Open, FileAccess.Read);
        }
        else
        {
          using (var client = new WebClient())
          {
            var data = client.DownloadData(path);
            bitmapImage.StreamSource = new MemoryStream(data);
          }
        }
      
        DecodePixelSize(parameter, bitmapImage);

        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
        bitmapImage.StreamSource.Dispose();

        lastLoadedImagePath = path;
        lastLoadedImage = bitmapImage;
      }

      return bitmapImage;
    }

    #region Convert

    public virtual object Convert(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      if (parameter is CacheImageParameters imageParameters)
      {
        DecodeHeight = imageParameters.DecodeHeight;
        DecodeWidth = imageParameters.DecodeWidth;
      }

      return GetBitmapImage(value, parameter);
    }

    #endregion
 
    #region RefreshDictionary

    public static void RefreshDictionary(string imagePath)
    {
      if (AllConverters.TryGetValue(imagePath, out var converters))
      {
        foreach (var converter in converters)
        {
          converter.lastLoadedImage = null;
          converter.lastLoadedImagePath = null;
        }

        AllConverters[imagePath] = null;
      }
    }

    #endregion

    protected virtual void  DecodePixelSize(object parameter, BitmapImage bitmapImage)
    {
      if (int.TryParse(parameter?.ToString(), out var pixelSize) || DecodeWidth != null || DecodeHeight != null)
      {
        if (pixelSize > 0)
          bitmapImage.DecodePixelWidth = pixelSize;

        if (pixelSize > 0)
          bitmapImage.DecodePixelHeight = pixelSize;

        if (DecodeWidth != null)
          bitmapImage.DecodePixelWidth = DecodeWidth.Value;

        if (DecodeHeight != null)
          bitmapImage.DecodePixelHeight = DecodeHeight.Value;
      }
    }

    private void AddConverterToDictionary()
    {

    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}