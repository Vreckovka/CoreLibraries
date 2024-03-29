﻿using System;
using System.ComponentModel;
using Prism.Regions;
using VCore.Standard.Modularity.Interfaces;

namespace VCore.WPF.Modularity.RegionProviders
{
  public interface IRegionProvider
  {
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
      where TViewModel : class, INotifyPropertyChanged, IActivable;
  

      void GoBack(Guid guid);

   
  }
}