using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VCore.WPF.Helpers;

namespace VCore.WPF.AttachedProperties
{
  public class AttachedProperty
  {
    #region RememberInitialWidth

    public static readonly DependencyProperty RememberInitialWidthProperty = DependencyProperty.RegisterAttached(
      "RememberInitialWidth",
      typeof(bool),
      typeof(AttachedProperty),
      new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender, OnRememberInitialSize)
    );

    private static void OnRememberInitialSize(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
      if ((bool)dependencyPropertyChangedEventArgs.NewValue)
      {
        ((FrameworkElement)dependencyObject).Loaded += AttachedProperty_Loaded;
      }
    }

    private static void AttachedProperty_Loaded(object sender, RoutedEventArgs e)
    {
      var frameworkElement = (FrameworkElement)sender;
      var isBubbleSource = GetRememberInitialWidth(frameworkElement);
      if (isBubbleSource)
      {
        frameworkElement.Width = frameworkElement.ActualWidth;
      }
    }

    public static void SetRememberInitialWidth(UIElement element, bool value)
    {
      element.SetValue(RememberInitialWidthProperty, value);
    }

    public static bool GetRememberInitialWidth(UIElement element)
    {
      return (bool)element.GetValue(RememberInitialWidthProperty);
    }

    #endregion

    #region ScrollSpeed

    public static double GetScrollSpeed(DependencyObject obj)
    {
      return (double)obj.GetValue(ScrollSpeedProperty);
    }

    public static void SetScrollSpeed(DependencyObject obj, double value)
    {
      obj.SetValue(ScrollSpeedProperty, value);
    }

    public static readonly DependencyProperty ScrollSpeedProperty =
      DependencyProperty.RegisterAttached(
        "ScrollSpeed",
        typeof(double),
        typeof(AttachedProperty),
        new FrameworkPropertyMetadata(
          1.0,
          FrameworkPropertyMetadataOptions.Inherits & FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
          new PropertyChangedCallback(OnScrollSpeedChanged)));

    public static DependencyObject GetScrollViewer(DependencyObject o)
    {
      // Return the DependencyObject if it is a ScrollViewer
      if (o is ScrollViewer)
      {
        return o;
      }

      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
      {
        var child = VisualTreeHelper.GetChild(o, i);

        var result = GetScrollViewer(child);
        if (result == null)
        {
          continue;
        }
        else
        {
          return result;
        }
      }

      return null;
    }

    private static void OnScrollSpeedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      var host = o as UIElement;
      host.PreviewMouseWheel += new MouseWheelEventHandler(OnPreviewMouseWheelScrolled);
    }

    private static void OnPreviewMouseWheelScrolled(object sender, MouseWheelEventArgs e)
    {
      DependencyObject scrollHost = sender as DependencyObject;

      double scrollSpeed = (double)(scrollHost).GetValue(AttachedProperty.ScrollSpeedProperty);

      ScrollViewer scrollViewer = GetScrollViewer(scrollHost) as ScrollViewer;

      if (scrollViewer != null)
      {
        double offset = scrollViewer.VerticalOffset - (e.Delta * scrollSpeed / 6);
        if (offset < 0)
        {
          scrollViewer.ScrollToVerticalOffset(0);
        }
        else if (offset > scrollViewer.ExtentHeight)
        {
          scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
        }
        else
        {
          scrollViewer.ScrollToVerticalOffset(offset);
        }

        e.Handled = true;
      }
      else
      {
        throw new NotSupportedException("ScrollSpeed Attached Property is not attached to an element containing a ScrollViewer.");
      }
    }

    #endregion

  }

  public class ScrollViewerCorrector
  {
    #region FixScrollingProperty

    public static readonly DependencyProperty FixScrollingProperty =
      DependencyProperty.RegisterAttached("FixScrolling", typeof(bool), typeof(ScrollViewerCorrector),
        new FrameworkPropertyMetadata(false, ScrollViewerCorrector.OnFixScrollingPropertyChanged));


    public static bool GetFixScrolling(DependencyObject obj)
    {
      return (bool)obj.GetValue(FixScrollingProperty);
    }

    public static void SetFixScrolling(DependencyObject obj, bool value)
    {
      obj.SetValue(FixScrollingProperty, value);
    }

    #endregion

    private static EventHandler listviewEventHandler;
    private static MouseWheelEventHandler scrollViewerEventHandler;

    public static void OnFixScrollingPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var dependencySender = ((FrameworkElement)sender);

      ScrollViewer viewer = null;
      if (dependencySender is ScrollViewer viewer1)
      {
        viewer = viewer1;
      }
      else
        viewer = dependencySender.GetFirstChildOfType<ScrollViewer>();

      if (viewer == null)
      {
        if (listviewEventHandler == null)
        {
          listviewEventHandler = new EventHandler((x, y) => ListView_LayoutUpdated(dependencySender, new EventArgs(), (bool)e.NewValue));
        }

        dependencySender.LayoutUpdated += listviewEventHandler;
      }


      if (viewer == null)
        return;

      HookToScrollViewer(viewer, (bool)e.NewValue, (UIElement)VisualTreeHelper.GetParent(dependencySender));

    }

    private static void ListView_LayoutUpdated(object sender, EventArgs e, bool hook)
    {
      var listview = (FrameworkElement)sender;

      listview.LayoutUpdated -= listviewEventHandler;

      var viewer = listview.GetFirstChildOfType<ScrollViewer>();

      if (viewer != null)
        HookToScrollViewer(viewer, hook, listview);

    }

    private static void HookToScrollViewer(ScrollViewer viewer, bool hook, UIElement owner)
    {
      if (scrollViewerEventHandler == null)
      {
        scrollViewerEventHandler = new MouseWheelEventHandler((x, y) => HandlePreviewMouseWheel(viewer, y, owner));
      }

      if (hook)
        viewer.PreviewMouseWheel += scrollViewerEventHandler;
      else
        viewer.PreviewMouseWheel -= scrollViewerEventHandler;
    }

    private static List<MouseWheelEventArgs> _reentrantList = new List<MouseWheelEventArgs>();

    private static void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e, UIElement owner)
    {
      var scrollControl = sender as ScrollViewer;

      if (!e.Handled && sender != null && !_reentrantList.Contains(e))
      {
        var previewEventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
          RoutedEvent = UIElement.PreviewMouseWheelEvent,
          Source = sender
        };

        var originalSource = e.OriginalSource as UIElement;

        _reentrantList.Add(previewEventArg);

        originalSource?.RaiseEvent(previewEventArg);

        _reentrantList.Remove(previewEventArg);


        if (!previewEventArg.Handled)
        {
          e.Handled = true;

          var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);

          eventArg.RoutedEvent = UIElement.MouseWheelEvent;

          eventArg.Source = sender;

          owner?.RaiseEvent(eventArg);

          scrollControl?.ScrollToVerticalOffset(scrollControl.VerticalOffset - (0.3 * e.Delta));
        }
      }
    }
  }
}
