﻿using System;
using System.Collections.Generic;
using Ninject;
using VCore.Standard;
using VCore.Standard.Modularity.Interfaces;
using VCore.WPF.Modularity.RegionProviders;
using VCore.WPF.ViewModels.Navigation;

namespace VCore.WPF.ViewModels
{
  public interface IRegionViewModel : IActivable, IInitializable
  {
    #region Properties

    bool ContainsNestedRegions { get; }
    string RegionName { get; }
    
    #endregion Properties
  }

  public abstract class RegionCollectionViewModel : ViewModel
  {
    #region Fields

    protected readonly IRegionProvider regionProvider;

    #endregion Fields

    #region Constructors

    public RegionCollectionViewModel(IRegionProvider regionProvider)
    {
      this.regionProvider = regionProvider ?? throw new ArgumentNullException(nameof(regionProvider));
    }

    #endregion Constructors

    #region Properties

    public abstract Dictionary<Type, Tuple<string, bool>> RegistredViews { get; set; }

    public Dictionary<Type, SelfActivableNavigationItem> Views { get; } = new Dictionary<Type, SelfActivableNavigationItem>();

    #endregion Properties

    #region Methods

    public void ActivateView(Type viewType)
    {
      Views[viewType].IsActive = true;
    }

    public override void Initialize()
    {
      base.Initialize();

      foreach (var registredView in RegistredViews)
      {
        var method = regionProvider.GetType().GetMethod("RegisterView");

        var genericMethod = method.MakeGenericMethod(registredView.Key, typeof(SelfActivableNavigationItem));

        var guid = genericMethod.Invoke(regionProvider, new object[] { registredView.Value.Item1, this, registredView.Value.Item2 });

        Views.Add(registredView.Key, new SelfActivableNavigationItem(regionProvider, (Guid)guid));
      }
    }

    #endregion Methods
  }


  public class SelfActivableNavigationItem : ViewModel, INavigationItem
  {
    #region Fields

    private readonly IRegionProvider regionProvider;

    #endregion Fields

    #region Constructors

    public SelfActivableNavigationItem(IRegionProvider regionProvider, Guid guid)
    {
      this.regionProvider = regionProvider ?? throw new ArgumentNullException(nameof(regionProvider));
      Guid = guid;
    }

    #endregion Constructors

    #region Properties

    public Guid Guid { get; internal set; }
    public string Header { get; set; }

    #region IsActive

    private bool isActive;
    private bool wasActivated;

    public bool IsActive
    {
      get { return isActive; }
      set
      {
        if (value != isActive)
        {
          isActive = value;

          if (isActive)
          {
            regionProvider.ActivateView(Guid);
          }
          else
          {
            regionProvider.DectivateView(Guid);
          }

          RaisePropertyChanged();
        }
      }
    }

    #endregion IsActive

    #endregion Properties
  }
}