using System;
using System.Windows;
using VCore.Standard.Modularity.Interfaces;

namespace VCore.WPF.Interfaces
{
  public interface IFilterable
  {
    void Filter(string predicated);
  }

  public interface IWPFView : IView
  {
    public event RoutedEventHandler Loaded;
    public event EventHandler Closed;

    public double Left { get; set; }
    public double Top { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }

    public double ActualHeight { get;  }

    public double ActualWidth { get;  }

  }
}