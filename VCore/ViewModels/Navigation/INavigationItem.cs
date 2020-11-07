using System;
using VCore.Standard.Modularity.Interfaces;

namespace VCore.ViewModels.Navigation
{
  public interface INavigationItem : IActivable, IDisposable
  {
    string Header { get; }
  }
}
