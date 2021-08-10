using System;
using Ninject;
using VCore.Standard;
using VCore.Standard.Modularity.Interfaces;

namespace VCore.ViewModels.Navigation
{
  public interface INavigationItem : IActivable, IInitializable, IDisposable
  {
    string Header { get; }
  }
}
