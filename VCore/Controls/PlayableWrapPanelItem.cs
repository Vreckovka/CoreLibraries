using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VCore.Controls
{
  public class PlayableWrapPanelItem : Control
  {
    #region HeaderText

    public string HeaderText
    {
      get { return (string)GetValue(HeaderTextProperty); }
      set { SetValue(HeaderTextProperty, value); }
    }

    public static readonly DependencyProperty HeaderTextProperty =
        DependencyProperty.Register(
            nameof(HeaderText),
            typeof(string),
            typeof(PlayableWrapPanelItem),
            new PropertyMetadata(null));


    #endregion

    #region BottomText

    public string BottomText
    {
      get { return (string)GetValue(BottomTextProperty); }
      set { SetValue(BottomTextProperty, value); }
    }

    public static readonly DependencyProperty BottomTextProperty =
        DependencyProperty.Register(
            nameof(BottomText),
            typeof(string),
            typeof(PlayableWrapPanelItem),
            new PropertyMetadata(null));


    #endregion

    #region BottomFaGlyph

    public string BottomFaGlyph
    {
      get { return (string)GetValue(BottomFaGlyphProperty); }
      set { SetValue(BottomFaGlyphProperty, value); }
    }

    public static readonly DependencyProperty BottomFaGlyphProperty =
      DependencyProperty.Register(
        nameof(BottomFaGlyph),
        typeof(string),
        typeof(PlayableWrapPanelItem),
        new PropertyMetadata(null));


    #endregion

    #region ImageThumbnail

    public ImageSource ImageThumbnail
    {
      get { return (ImageSource)GetValue(ImageThumbnailProperty); }
      set { SetValue(ImageThumbnailProperty, value); }
    }

    public static readonly DependencyProperty ImageThumbnailProperty =
        DependencyProperty.Register(
            nameof(ImageThumbnail),
            typeof(ImageSource),
            typeof(PlayableWrapPanelItem),
            new PropertyMetadata(null, (x, y) =>
            {
              var newValue = y.NewValue;
            }));


    #endregion

    #region IsPlaying

    public bool IsPlaying
    {
      get { return (bool)GetValue(IsPlayingProperty); }
      set { SetValue(IsPlayingProperty, value); }
    }

    public static readonly DependencyProperty IsPlayingProperty =
        DependencyProperty.Register(
            nameof(IsPlaying),
            typeof(bool),
            typeof(PlayableWrapPanelItem),
            new PropertyMetadata(null));


    #endregion


  }
}
