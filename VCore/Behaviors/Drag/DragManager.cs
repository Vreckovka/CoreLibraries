using System.Linq;
using System.Windows;
using System.Windows.Documents;
using VCore.Helpers;
using VCore.WPF.Behaviors.Listview;

namespace VCore.WPF.Behaviors.Drag
{
  public static class DragManager
  {
    #region The dependecy Property

    private static readonly DependencyProperty ShowDragProperty = DependencyProperty.RegisterAttached("ShowDrag", typeof(bool), typeof(DragManager), new PropertyMetadata(PreviewDropCommandPropertyChangedCallBack));

    #endregion



    public static void SetShowDrag(this UIElement inUIElement, bool inCommand)
    {
      inUIElement.SetValue(ShowDragProperty, inCommand);
    }

    private static bool GetShowDrag(UIElement inUIElement)
    {
      return (bool)inUIElement.GetValue(ShowDragProperty);
    }


    private static void PreviewDropCommandPropertyChangedCallBack(DependencyObject inDependencyObject, DependencyPropertyChangedEventArgs inEventArgs)
    {
      if ((bool)inDependencyObject.GetValue(ShowDragProperty))
      {
        UIElement uiElement = inDependencyObject as UIElement;

        if (null == uiElement) return;


        AdornedControl.AdornedControl adornerControl = null;

        uiElement.DragEnter += (sender, args) =>
        {
          if (adornerControl == null)
            adornerControl = uiElement.GetFirstParentOfType<AdornedControl.AdornedControl>();

          if (adornerControl != null)
            adornerControl.IsAdornerVisible = true;
        };

        uiElement.DragLeave += (sender, args) =>
        {
          if (adornerControl == null)
            adornerControl = uiElement.GetFirstParentOfType<AdornedControl.AdornedControl>();

          if (adornerControl != null)
            adornerControl.IsAdornerVisible = false;

        };

        uiElement.Drop += (sender, args) =>
        {
          if (adornerControl == null)
            adornerControl = uiElement.GetFirstParentOfType<AdornedControl.AdornedControl>();

          if (adornerControl != null)
            adornerControl.IsAdornerVisible = false;
        };
      }
    }

    #region RemoveAdorner

    private static void RemoveAdorner(UIElement uIElement)
    {
      var adorner = AdornerLayer.GetAdornerLayer(uIElement);

      if (adorner != null)
      {
        var adorners = adorner.GetAdorners(uIElement);

        if (adorners != null)
        {
          var actualAdorner = adorners.FirstOrDefault();

          if (actualAdorner != null)
          {
            adorner.Remove(actualAdorner);
          }
        }
      }
    }

    #endregion



  }
}