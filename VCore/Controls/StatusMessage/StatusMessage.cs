using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VCore.Controls;

namespace VCore.WPF.Controls.StatusMessage
{
  public enum StatusType
  {
    Starting,
    Processing,
    Failed,
    Done,
    Error,
  }

  public class StatusMessage : Control
  {
    public StatusMessage()
    {
      DataContextChanged += StatusMessage_DataContextChanged;
    }

    #region Status

    public StatusType Status
    {
      get { return (StatusType)GetValue(StatusProperty); }
      set { SetValue(StatusProperty, value); }
    }

    public static readonly DependencyProperty StatusProperty =
      DependencyProperty.Register(
        nameof(Status),
        typeof(StatusType),
        typeof(StatusMessage),
        new PropertyMetadata((x, y) =>
        {
          if (x is StatusMessage statusMessage)
          {
            var newValue = (StatusType)y.NewValue;
            var token = statusMessage.cts?.Token;

            if (token == null)
            {
              statusMessage.cts = new CancellationTokenSource();
              token = statusMessage.cts.Token;
            }

            if (newValue == StatusType.Done)
            {

              statusMessage.HideStatusMessage(2000, token.Value);
            }
            else if (newValue == StatusType.Error)
            {
              statusMessage.HideStatusMessage(4000, token.Value);
            }
            else if (newValue == StatusType.Processing ||
                     newValue == StatusType.Starting)
            {
              statusMessage.ShowStatusMessage();
            }
          }
        }));


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
        typeof(StatusMessage),
        new PropertyMetadata(null));

    #endregion

    #region IsPinned

    public bool IsPinned
    {
      get { return (bool)GetValue(IsPinnedProperty); }
      set { SetValue(IsPinnedProperty, value); }
    }

    public static readonly DependencyProperty IsPinnedProperty =
      DependencyProperty.Register(
        nameof(IsPinned),
        typeof(bool),
        typeof(StatusMessage),
        new PropertyMetadata(false, (x, y) =>
        {
          if (x is StatusMessage statusMessage)
          {
            var value = (bool)y.NewValue;

            if (value)
            {
              var doubleAnimation = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(0)));
              statusMessage.BeginAnimation(OpacityProperty, doubleAnimation);
            }
            else if (statusMessage.shouldBeHidden)
            {
              var token = statusMessage.cts?.Token;

              if (token == null)
              {
                statusMessage.cts = new CancellationTokenSource();
                token = statusMessage.cts.Token;
              }

              statusMessage.HideStatusMessage(1000, token.Value);
            }
          }
        }));


    #endregion

    #region AnimationDuration

    public Duration AnimationDuration
    {
      get { return (Duration)GetValue(AnimationDurationProperty); }
      set { SetValue(AnimationDurationProperty, value); }
    }

    public static readonly DependencyProperty AnimationDurationProperty =
      DependencyProperty.Register(
        nameof(AnimationDuration),
        typeof(Duration),
        typeof(StatusMessage),
        new PropertyMetadata(new Duration(TimeSpan.FromSeconds(0.5))));


    #endregion

    #region Methods

    #region StatusMessage_DataContextChanged

    private void StatusMessage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (Status < StatusType.Failed)
        ShowStatusMessage();
      else
      {
        var token = cts?.Token;

        if (token == null)
        {
          cts = new CancellationTokenSource();
          token = cts.Token;
        }

        ShowStatusMessage();
        HideStatusMessage(4000, cts.Token);
      }
    }

    #endregion

    #region HideStatusMessage

    private bool shouldBeHidden = false;
    public async void HideStatusMessage(int delay, CancellationToken cancellationToken)
    {
      await Task.Delay(delay);

      if (IsPinned != true && !cancellationToken.IsCancellationRequested)
      {
        var doubleAnimation = new DoubleAnimation(0, AnimationDuration);

        this.BeginAnimation(OpacityProperty, doubleAnimation);
      }

      shouldBeHidden = true;
    }

    #endregion

    #region ShowStatusMessage

    private CancellationTokenSource cts;
    public void ShowStatusMessage()
    {
      cts?.Cancel();
      cts = new CancellationTokenSource();

      shouldBeHidden = false;

      var doubleAnimation = new DoubleAnimation(1, AnimationDuration);
      this.BeginAnimation(OpacityProperty, doubleAnimation);
    }

    #endregion 

    #endregion
  }
}
