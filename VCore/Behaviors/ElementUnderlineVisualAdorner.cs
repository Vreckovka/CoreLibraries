using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace VCore.WPF.Behaviors
{
  public class ElementUnderlineVisualAdorner : Adorner
  {
    public ElementUnderlineVisualAdorner(UIElement adornedElement)
      : base(adornedElement)
    {
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      Rect adornedElementRect = new Rect(this.AdornedElement.RenderSize);

      Pen renderPen = new Pen((SolidColorBrush)(new BrushConverter().ConvertFrom("#df0e71")), 1);

      drawingContext.DrawLine(renderPen, adornedElementRect.BottomLeft, adornedElementRect.BottomRight);

    }
  }
}