using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using VCore.WPF.Behaviors.Listview;

namespace VCore.WPF.Behaviors.Drag
{
  public class DragBehavior : Behavior<FrameworkElement>
  {
    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.DragEnter += AssociatedObject_DragEnter;
      AssociatedObject.DragOver += AssociatedObject_DragOver;
      AssociatedObject.DragLeave += AssociatedObject_DragLeave;
      AssociatedObject.PreviewMouseMove += AssociatedObject_PreviewMouseMove;
     
    }

    private void AssociatedObject_DragLeave(object sender, DragEventArgs e)
    {
    }

    private void AssociatedObject_DragOver(object sender, DragEventArgs e)
    {
    }

    private void AssociatedObject_DragEnter(object sender, DragEventArgs e)
    {
     
    }

    #region AssociatedObject_PreviewMouseMove

    private void AssociatedObject_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
      {
        DragDrop.DoDragDrop(AssociatedObject, AssociatedObject.DataContext, DragDropEffects.All);
      }
    }

    #endregion
  }
}