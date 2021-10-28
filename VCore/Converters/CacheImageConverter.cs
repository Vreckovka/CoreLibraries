using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace VCore.WPF.Converters
{
  public class CacheImageParameters
  {
    public int? DecodeHeight { get; set; }
    public int? DecodeWidth { get; set; }
  }

  public class CacheImageConverter : BaseConverter
  {
    public int? DecodeWidth { get; set; }
    public int? DecodeHeight { get; set; }

    public static Dictionary<string, List<CacheImageConverter>> AllConverters = new Dictionary<string, List<CacheImageConverter>>();
    string lastLoadedImagePath;
    BitmapImage lastLoadedImage;

    #region Convert

    public override object Convert(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      if (value == null)
      {
        return null;
      }

      if (parameter is CacheImageParameters imageParameters)
      {
        DecodeHeight = imageParameters.DecodeHeight;
        DecodeWidth = imageParameters.DecodeWidth;
      }

      var stringValue = value?.ToString();

      if (stringValue != null && stringValue.Contains("http"))
      {
        return stringValue;
      }

      return GetBitmapImage(value, parameter);
    }

    #endregion

    #region GetBitmapImage

    protected BitmapImage GetBitmapImage(object value, object parameter)
    {
      try
      {
        if (value is BitmapImage bitmapImage1)
        {
          return bitmapImage1;
        }

        string path = "";

        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();

        if (value != DependencyProperty.UnsetValue && value != null)
        {
          path = value.ToString()?.Replace("file:///", "");
        }
        else
        {
          bitmapImage.StreamSource = GetEmptyImage();
        }

        if (File.Exists(path) && path != null)
        {
          bitmapImage.StreamSource = new FileStream(path, FileMode.Open, FileAccess.Read);
        }
        else
        {
          bitmapImage.StreamSource = GetEmptyImage();
        }

        if (lastLoadedImagePath == path)
        {
          return lastLoadedImage;
        }


        DecodePixelSize(parameter, bitmapImage);

        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
        bitmapImage.StreamSource.Dispose();

        lastLoadedImagePath = path;
        lastLoadedImage = bitmapImage;

        if (!AllConverters.ContainsKey(path))
        {
          AllConverters.Add(path, new List<CacheImageConverter>()
        {
          this
        });
        }
        else
        {
          var allConvertes = AllConverters[path];

          if (allConvertes == null)
          {
            allConvertes = new List<CacheImageConverter>()
          {
            this
          };
          }
          else if (!allConvertes.Contains(this))
          {
            allConvertes.Add(this);
          }

          AllConverters[path] = allConvertes;
        }


        return bitmapImage;
      }
      catch (Exception)
      {
        return null;
      }
    }

    #endregion

    #region GetEmptyImage

    protected virtual Stream GetEmptyImage()
    {
      var stream = new MemoryStream();
      var emptyImage = new Bitmap(10, 10, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      emptyImage.Save(stream, ImageFormat.Jpeg);

      return stream;
    }

    #endregion

    #region RefreshDictionary

    public static void RefreshDictionary(string imagePath)
    {
      if (AllConverters.TryGetValue(imagePath, out var converters))
      {
        if (converters != null)
        {
          foreach (var converter in converters)
          {
            converter.lastLoadedImage = null;
            converter.lastLoadedImagePath = null;
          }

          AllConverters[imagePath] = null;
        }
      }
    }

    #endregion

    #region DecodePixelSize

    protected virtual void DecodePixelSize(object parameter, BitmapImage bitmapImage)
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

    #endregion

  }
}