﻿using System;
using System.ComponentModel;
using System.Windows;
using Prism.Regions;
using VCore.Standard.Factories.ViewModels;
using VCore.Standard.Factories.Views;
using VCore.Standard.Modularity.Interfaces;

namespace VCore.WPF.Modularity.RegionProviders
{
  public class RegistredView<TView, TViewModel> : IDisposable, IRegistredView
    where TView : class, IView
    where TViewModel : class, INotifyPropertyChanged, IActivable
  {

    #region Fields

    private readonly IBaseFactory baseFactory;
    private readonly IViewModelsFactory viewModelsFactory;

    #endregion Fields

    #region Constructors

    public RegistredView(
      IRegion region,
      IBaseFactory baseFactory,
      IViewModelsFactory viewModelsFactory,
      TViewModel viewModel = null
    ) : this(region, baseFactory, viewModelsFactory, viewModel, false)
    {
    }

    public RegistredView(
      IRegion region,
      IBaseFactory baseFactory,
      IViewModelsFactory viewModelsFactory,
      TViewModel viewModel = null,
      bool initializeImmediately = false
    )
    {
      this.baseFactory = baseFactory ?? throw new ArgumentNullException(nameof(baseFactory));
      this.viewModelsFactory = viewModelsFactory ?? throw new ArgumentNullException(nameof(viewModelsFactory));

      Guid = Guid.NewGuid();

      ViewModel = viewModel;
      ViewName = GetViewName(region.Name, ViewModel);
      Region = region;

      if (initializeImmediately)
      {
        View = RegisterView(initializeImmediately);
      }
    }

    #endregion Constructors

    #region Properties

    public Guid Guid { get; }

    #region Region

    public IRegion region;
    public IRegion Region
    {
      get { return region; }
      set
      {
        if (value != region)
        {
          region = value;
          if (!Region.Views.Contains(View) && View != null)
          {
            Region.Add(View, ViewName);
          }
        }
      }
    }

    #endregion

    public TView View { get; set; }
    public string ViewName { get; set; }
    

    public IRegionManager RegionManager { get; set; }

    #region ViewModel

    private TViewModel viewModel;

    public TViewModel ViewModel
    {
      get { return viewModel; }
      set
      {
        if (value != viewModel)
        {
          viewModel = value;

          if (View != null)
          {
            View.DataContext = viewModel;

            Refresh();
          }   
        }
      }
    }

    #endregion ViewModel

    #endregion Properties

    #region Methods

    #region Activate

    public void Activate()
    {
      Region.Activate(RegisterView());

      if (ViewModel is IActivable activable && !activable.IsActive)
      {
        activable.IsActive = true;
      }
    }

    #endregion Activate

    #region Deactivate

    public void Deactivate()
    {
      Region.Deactivate(View);

      if (ViewModel is IActivable activable)
      {
        activable.IsActive = false;
      }

    }

    #endregion

    #region DeactivateViewModel

    public void DeactivateDataContext()
    {
      if (ViewModel is IActivable activable)
      {
        activable.IsActive = false;
      }
    }

    #endregion

    #region Refresh

    public void Refresh()
    {
      var newView = Create();
      newView.DataContext = View.DataContext;

      Region.Remove(View);

      Region.Add(newView);

      View = newView;
    }

    #endregion

    #region RegisterView

    public TView RegisterView(bool createScope = false)
    {

      if (View == null)
      {
        var view = Region.GetView(ViewName);

        if (view != null)
        {
          Region.Remove(view);
        }


        View = Create();

        if (ViewModel == null)
        {
          ViewModel = viewModelsFactory.Create<TViewModel>();
        }

        RegionManager = Region.Add(View, ViewName, createScope);

        View.DataContext = ViewModel;


      }

      return View;
    }

    #endregion RegisterView

    #region Create

    public TView Create()
    {
      return Application.Current?.Dispatcher?.Invoke(() =>
      {
        return baseFactory.Create<TView>();
      });
    }

    #endregion Create

    #region GetViewName

    public static string GetViewName(string regionName, object viewModel)
    {
      return typeof(TView).Name + "_" + viewModel + "_" + regionName;
    }

    #endregion GetViewName

    #region ToString

    public override string ToString()
    {
      return ViewName;
    }

    #endregion ToString

    #region Dispose

    public void Dispose()
    {
     
    }

    #endregion Dispose

    #endregion 

  }
}