using System;
using Ninject;
using VCore.Standard.Modularity.Interfaces;

namespace VCore.WPF.ViewModels.Navigation
{
  public interface INavigationItem : IActivable, IInitializable, IDisposable
  {
    string Header { get; }
  }
}
