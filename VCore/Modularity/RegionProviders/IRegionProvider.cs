using System;
using System.ComponentModel;
using Prism.Regions;
using VCore.Standard.Modularity.Interfaces;

namespace VCore.Modularity.RegionProviders
{
  public interface IRegionProvider
  {
    public IRegionManager RegionManager { get; set; }

    #region Methods

    void ActivateView(Guid guidObject);
    void RefreshView(Guid guidObject);
    void DectivateView(Guid guidObject);

    IRegionManager RegisterView<TView, TViewModel>(
      string regionName,
      TViewModel viewModel,
      bool containsNestedRegion,
      out Guid guid,
      IRegionManager regionManager = null)
      where TView : class, IView
      where TViewModel : class, INotifyPropertyChanged;
  

      void GoBack(Guid guid);

    #endregion Methods
  }
}