using System.Windows;
using System.Windows.Controls;

namespace VCore.WPF.Controls.LoadingIndicator
{
  public class LoadingIndicator : ContentControl
  {
    #region Progress

    public double? Progress
    {
      get { return (double?)GetValue(ProgressProperty); }
      set { SetValue(ProgressProperty, value); }
    }

    public static readonly DependencyProperty ProgressProperty =
      DependencyProperty.Register(
        nameof(Progress),
        typeof(double?),
        typeof(LoadingIndicator),
        new PropertyMetadata(null));

    #endregion

    #region ProcessedCount

    public double? ProcessedCount
    {
      get { return (double?)GetValue(ProcessedCountProperty); }
      set { SetValue(ProcessedCountProperty, value); }
    }

    public static readonly DependencyProperty ProcessedCountProperty =
      DependencyProperty.Register(
        nameof(ProcessedCount),
        typeof(double?),
        typeof(LoadingIndicator),
        new PropertyMetadata(null));

    #endregion

    #region NumberOfProcesses

    public double? NumberOfProcesses
    {
      get { return (double?)GetValue(NumberOfProcessesProperty); }
      set { SetValue(NumberOfProcessesProperty, value); }
    }

    public static readonly DependencyProperty NumberOfProcessesProperty =
      DependencyProperty.Register(
        nameof(NumberOfProcesses),
        typeof(double?),
        typeof(LoadingIndicator),
        new PropertyMetadata(null));

    #endregion

    #region MessageDataContext

    public object MessageDataContext
    {
      get { return (object)GetValue(MessageDataContextProperty); }
      set { SetValue(MessageDataContextProperty, value); }
    }

    public static readonly DependencyProperty MessageDataContextProperty =
      DependencyProperty.Register(
        nameof(MessageDataContext),
        typeof(object),
        typeof(LoadingIndicator),
        new PropertyMetadata(null));

    #endregion

    #region Message

    public string Message
    {
      get { return (string)GetValue(MessageProperty); }
      set { SetValue(MessageProperty, value); }
    }

    public static readonly DependencyProperty MessageProperty =
      DependencyProperty.Register(
        nameof(Message),
        typeof(string),
        typeof(LoadingIndicator),
        new PropertyMetadata(null));

    #endregion

    #region IsLoading

    public bool IsLoading
    {
      get { return (bool)GetValue(IsLoadingProperty); }
      set { SetValue(IsLoadingProperty, value); }
    }

    public static readonly DependencyProperty IsLoadingProperty =
      DependencyProperty.Register(
        nameof(IsLoading),
        typeof(bool),
        typeof(LoadingIndicator),
        new PropertyMetadata(false));

    #endregion
  }
}
