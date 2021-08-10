using System.Windows;
using System.Windows.Input;

namespace VCore.WPF.Behaviors.Drag
{
  /// <summary>
  /// This is an Attached Behavior and is intended for use with
  /// XAML objects to enable binding a drag and drop event to
  /// an ICommand.
  /// </summary>
  public static class DropBehavior
  {
    #region The dependecy Property
    /// <summary>
    /// The Dependency property. To allow for Binding, a dependency
    /// property must be used.
    /// </summary>
    private static readonly DependencyProperty PreviewDropCommandProperty =
                DependencyProperty.RegisterAttached
                (
                    "PreviewDropCommand",
                    typeof(ICommand),
                    typeof(DropBehavior),
                    new PropertyMetadata(PreviewDropCommandPropertyChangedCallBack)
                );
    #endregion

    #region The getter and setter
 
    public static void SetPreviewDropCommand(this UIElement inUIElement, ICommand inCommand)
    {
      inUIElement.SetValue(PreviewDropCommandProperty, inCommand);
    }

    private static ICommand GetPreviewDropCommand(UIElement inUIElement)
    {
      return (ICommand)inUIElement.GetValue(PreviewDropCommandProperty);
    }

    #endregion

    #region The PropertyChangedCallBack method
   
    private static void PreviewDropCommandPropertyChangedCallBack(DependencyObject inDependencyObject, DependencyPropertyChangedEventArgs inEventArgs)
    {
      UIElement uiElement = inDependencyObject as UIElement;
      if (null == uiElement) return;

      uiElement.Drop += (sender, args) =>
      {
        GetPreviewDropCommand(uiElement).Execute(args.Data);
        args.Handled = true;
      };
    }

    #endregion
  }
}
