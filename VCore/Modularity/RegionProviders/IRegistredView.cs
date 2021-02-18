using System;
using System.Reactive.Subjects;
using Prism.Regions;

namespace VCore.Modularity.RegionProviders
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