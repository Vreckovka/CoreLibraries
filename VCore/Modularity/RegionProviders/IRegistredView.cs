using System;
using Prism.Regions;

namespace VCore.WPF.Modularity.RegionProviders
{
  public interface IRegistredView
  {
    Guid Guid { get; }
    string ViewName { get; set; }
    IRegion Region { get; set; }

    void Refresh();
    void Activate();
    void Deactivate();

    void DeactivateDataContext();
  }
}