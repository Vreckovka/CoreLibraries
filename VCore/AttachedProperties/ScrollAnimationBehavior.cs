using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using VCore.Controls;
using VCore.Helpers;

namespace ScrollAnimateBehavior.AttachedBehaviors
{
  public static class ScrollAnimationBehavior
  {
    #region Private ScrollViewer for ListBox

    private static ScrollViewer _listBoxScroller = new ScrollViewer();

    #endregion

    #region VerticalOffset Property

    public static DependencyProperty VerticalOffsetProperty =
        DependencyProperty.RegisterAttached("VerticalOffset",
                                            typeof(double),
                                            typeof(ScrollAnimationBehavior),
                                            new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));

    public static void SetVerticalOffset(FrameworkElement target, double value)
    {
      target.SetValue(VerticalOffsetProperty, value);
    }

    public static double GetVerticalOffset(FrameworkElement target)
    {
      return (double)target.GetValue(VerticalOffsetProperty);
    }

    #endregion

    #region TimeDuration Property

    public static DependencyProperty TimeDurationProperty =
        DependencyProperty.RegisterAttached("TimeDuration",
                                            typeof(TimeSpan),
                                            typeof(ScrollAnimationBehavior),
                                            new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 0)));

    public static void SetTimeDuration(FrameworkElement target, TimeSpan value)
    {
      target.SetValue(TimeDurationProperty, value);
    }

    public static TimeSpan GetTimeDuration(FrameworkElement target)
    {
      return (TimeSpan)target.GetValue(TimeDurationProperty);
    }

    #endregion

    #region PointsToScroll Property

    public static DependencyProperty PointsToScrollProperty =
        DependencyProperty.RegisterAttached("PointsToScroll",
                                            typeof(double),
                                            typeof(ScrollAnimationBehavior),
                                            new PropertyMetadata(0.0));

    public static void SetPointsToScroll(FrameworkElement target, double value)
    {
      target.SetValue(PointsToScrollProperty, value);
    }

    public static double GetPointsToScroll(FrameworkElement target)
    {
      return (double)target.GetValue(PointsToScrollProperty);
    }

    #endregion

    #region OnVerticalOffset Changed

    private static void OnVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
      ScrollViewer scrollViewer = target as ScrollViewer;

      if (scrollViewer != null)
      {
        scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
      }
    }

    #endregion

    #region IsEnabled Property

    public static DependencyProperty IsEnabledProperty =
                                            DependencyProperty.RegisterAttached("IsEnabled",
                                            typeof(bool),
                                            typeof(ScrollAnimationBehavior),
                                            new UIPropertyMetadata(false, OnIsEnabledChanged));

    public static void SetIsEnabled(FrameworkElement target, bool value)
    {
      target.SetValue(IsEnabledProperty, value);
    }

    public static bool GetIsEnabled(FrameworkElement target)
    {
      return (bool)target.GetValue(IsEnabledProperty);
    }

    #endregion

    #region OnIsEnabledChanged Changed

    private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var target = sender;

      if (target != null && target is ScrollViewer)
      {
        ScrollViewer scroller = target as ScrollViewer;
        scroller.Loaded += new RoutedEventHandler(scrollerLoaded);
      }

      if (target != null && target is ListBox)
      {
        ListBox listbox = target as ListBox;
        listbox.Loaded += new RoutedEventHandler(listboxLoaded);
      }
    }

    #endregion

    #region AnimateScroll

    public static void AnimateScroll(ScrollViewer scrollViewer, double ToValue)
    {
      DoubleAnimation verticalAnimation = new DoubleAnimation();

      verticalAnimation.From = scrollViewer.VerticalOffset;
      verticalAnimation.To = ToValue;
      verticalAnimation.Duration = new Duration(GetTimeDuration(scrollViewer));

      Storyboard storyboard = new Storyboard();

      storyboard.Children.Add(verticalAnimation);
      Storyboard.SetTarget(verticalAnimation, scrollViewer);
      Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(ScrollAnimationBehavior.VerticalOffsetProperty));
      storyboard.Begin();
    }

    #endregion

    #region NormalizeScrollPos Helper

    private static double NormalizeScrollPos(ScrollViewer scroll, double scrollChange, Orientation o)
    {
      double returnValue = scrollChange;

      if (scrollChange < 0)
      {
        returnValue = 0;
      }

      if (o == Orientation.Vertical && scrollChange > scroll.ScrollableHeight)
      {
        returnValue = scroll.ScrollableHeight;
      }
      else if (o == Orientation.Horizontal && scrollChange > scroll.ScrollableWidth)
      {
        returnValue = scroll.ScrollableWidth;
      }

      return returnValue;
    }

    #endregion

    #region UpdateScrollPosition Helper

    private static void UpdateScrollPosition(object sender)
    {
      ListBox listbox = sender as ListBox;

      if (listbox != null)
      {
        double scrollTo = 0;

        for (int i = 0; i < (listbox.SelectedIndex); i++)
        {
          ListBoxItem tempItem = listbox.ItemContainerGenerator.ContainerFromItem(listbox.Items[i]) as ListBoxItem;

          if (tempItem != null)
          {
            scrollTo += tempItem.ActualHeight;
          }
        }

        AnimateScroll(_listBoxScroller, scrollTo);
      }
    }

    #endregion

    private static Dictionary<ScrollViewer, bool> isDraggingThumb = new Dictionary<ScrollViewer, bool>();

    #region SetEventHandlersForScrollViewer Helper

    private static void SetEventHandlersForScrollViewer(ScrollViewer scroller)
    {
      //scroller.PreviewMouseWheel += new MouseWheelEventHandler(ScrollViewerPreviewMouseWheel);
      scroller.PreviewKeyDown += new KeyEventHandler(ScrollViewerPreviewKeyDown);
      scroller.ScrollChanged += Scroller_ScrollChanged;


      var thumb = scroller.GetFirstChildOfType<Thumb>();

      if (thumb != null)
      {
        thumb.DragStarted += Thumb_DragStarted;
        thumb.DragCompleted += Thumb_DragCompleted; ;
      }

    }

    private static void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
      if (sender is Thumb thumb)
      {
        var scrollViewer = thumb.GetFirstParentOfType<ScrollViewer>();

        if (scrollViewer != null)
        {
          if (isDraggingThumb.TryGetValue(scrollViewer, out var isDragging))
          {
            isDraggingThumb[scrollViewer] = false;
          }
          else
          {
            isDraggingThumb.Add(scrollViewer, false);
          }
        }
      }
    }

    private static void Thumb_DragStarted(object sender, DragStartedEventArgs e)
    {
      if (sender is Thumb thumb)
      {
        var scrollViewer = thumb.GetFirstParentOfType<ScrollViewer>();

        if (scrollViewer != null)
        {
          if (isDraggingThumb.TryGetValue(scrollViewer, out var isDragging))
          {
            isDraggingThumb[scrollViewer] = true;
          }
          else
          {
            isDraggingThumb.Add(scrollViewer, true);
          }
        }
      }
    }


    private static void Scroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if (sender is ScrollViewer scrollViewer)

        Application.Current.Dispatcher.Invoke(() =>
        {
          if (isDraggingThumb.TryGetValue(scrollViewer, out var isDragging))
          {
            if (isDragging)
            {
              scrollViewer.BeginAnimation(ScrollAnimationBehavior.VerticalOffsetProperty, null);
              scrollViewer.SetValue(ScrollAnimationBehavior.VerticalOffsetProperty, e.VerticalOffset);
              var value = scrollViewer.GetValue(ScrollAnimationBehavior.VerticalOffsetProperty);
            }
             
          }
        });
    }

    #endregion

    #region scrollerLoaded Event Handler

    private static void scrollerLoaded(object sender, RoutedEventArgs e)
    {
      ScrollViewer scroller = sender as ScrollViewer;

      SetEventHandlersForScrollViewer(scroller);
    }

    #endregion

    #region listboxLoaded Event Handler

    private static void listboxLoaded(object sender, RoutedEventArgs e)
    {
      ListBox listbox = sender as ListBox;

      _listBoxScroller = listbox.GetFirstChildOfType<ScrollViewer>();

      SetEventHandlersForScrollViewer(_listBoxScroller);

      SetTimeDuration(_listBoxScroller, new TimeSpan(0, 0, 0, 0, 200));
      SetPointsToScroll(_listBoxScroller, 16.0);

      //listbox.SelectionChanged += new SelectionChangedEventHandler(ListBoxSelectionChanged);
      listbox.Loaded += new RoutedEventHandler(ListBoxLoaded);
      listbox.LayoutUpdated += new EventHandler(ListBoxLayoutUpdated);
    }

    #endregion

    #region ScrollViewerPreviewMouseWheel Event Handler

    private static void ScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      double mouseWheelChange = (double)e.Delta;
      ScrollViewer scroller = (ScrollViewer)sender;

      double newVOffset = GetVerticalOffset(scroller) - (mouseWheelChange / 3);

      if (newVOffset < 0)
      {
        AnimateScroll(scroller, 0);
      }
      else if (newVOffset > scroller.ScrollableHeight)
      {
        AnimateScroll(scroller, scroller.ScrollableHeight);
      }
      else
      {
        AnimateScroll(scroller, newVOffset);
      }

      e.Handled = true;
    }

    #endregion

    #region ScrollViewerPreviewKeyDown Handler

    private static void ScrollViewerPreviewKeyDown(object sender, KeyEventArgs e)
    {
      ScrollViewer scroller = (ScrollViewer)sender;

      Key keyPressed = e.Key;
      double newVerticalPos = GetVerticalOffset(scroller);
      bool isKeyHandled = false;

      if (keyPressed == Key.Down)
      {
        newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos + GetPointsToScroll(scroller)), Orientation.Vertical);
        isKeyHandled = true;
      }
      else if (keyPressed == Key.PageDown)
      {
        newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos + scroller.ViewportHeight), Orientation.Vertical);
        isKeyHandled = true;
      }
      else if (keyPressed == Key.Up)
      {
        newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos - GetPointsToScroll(scroller)), Orientation.Vertical);
        isKeyHandled = true;
      }
      else if (keyPressed == Key.PageUp)
      {
        newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos - scroller.ViewportHeight), Orientation.Vertical);
        isKeyHandled = true;
      }

      if (newVerticalPos != GetVerticalOffset(scroller))
      {
        AnimateScroll(scroller, newVerticalPos);
      }

      e.Handled = isKeyHandled;
    }

    #endregion

    #region ListBox Event Handlers

    private static void ListBoxLayoutUpdated(object sender, EventArgs e)
    {
      UpdateScrollPosition(sender);
    }

    private static void ListBoxLoaded(object sender, RoutedEventArgs e)
    {
      UpdateScrollPosition(sender);
    }

    private static void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      UpdateScrollPosition(sender);
    }

    #endregion
  }



}