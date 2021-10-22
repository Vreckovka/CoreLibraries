using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Prism.Regions;
using VCore.Standard.Factories.ViewModels;
using VCore.Standard.Factories.Views;
using VCore.Standard.Modularity.Interfaces;
using VCore.WPF.Helpers;
using VCore.WPF.Modularity.Navigation;

namespace VCore.WPF.Modularity.RegionProviders
{
  public class RegionProvider : IRegionProvider
  {
    #region Fields

    private List<IRegionManager> regionManagers = new List<IRegionManager>();
    protected readonly IViewModelsFactory viewModelsFactory;
    private readonly INavigationProvider navigationProvider;
    private readonly IViewFactory viewFactory;
    private List<IRegistredView> Views = new List<IRegistredView>();
    private Dictionary<IRegistredView, IDisposable> ActivateSubscriptions = new Dictionary<IRegistredView, IDisposable>();

    #endregion Fields

    #region Constructors

    public RegionProvider(
      IRegionManager regionManager,
      IViewFactory viewFactory,
      IViewModelsFactory viewModelsFactory,
      INavigationProvider navigationProvider)
    {
      this.viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
      this.viewModelsFactory = viewModelsFactory ?? throw new ArgumentNullException(nameof(viewModelsFactory));
      this.navigationProvider = navigationProvider ?? throw new ArgumentNullException(nameof(navigationProvider));

      regionManagers.Add(regionManager);
    }

    #endregion Constructors

    #region Methods

    #region SubscribeToChanges

    private void SubscribeToChanges<TView, TViewModel>(RegistredView<TView, TViewModel> view)
      where TView : class, IView
      where TViewModel : class, INotifyPropertyChanged, IActivable
    {
      var disposable = view.ViewModel.ObservePropertyChange(x => x.IsActive).Where(x => x).Subscribe((x) =>
      {
        navigationProvider.Navigate(view);
      }, err => throw err);

      if (!ActivateSubscriptions.TryGetValue(view, out var subscription))
      {
        ActivateSubscriptions.Add(view, disposable);
      }
      else
      {
        subscription.Dispose();
        ActivateSubscriptions[view] = disposable;
      }
    }

    #endregion SubscribeToChanges

    #region RegisterView

    public IRegionManager RegisterView<TView, TViewModel>(
    string regionName,
    TViewModel viewModel,
    bool containsNestedRegion,
    out Guid guid,
    IRegionManager parentRegionManager = null)
    where TView : class, IView
    where TViewModel : class, INotifyPropertyChanged, IActivable
    {
      var registredView = Views.SingleOrDefault(x => x.ViewName == RegistredView<TView, TViewModel>.GetViewName(regionName, viewModel));

      if (registredView == null)
      {
        IRegionManager actualRegionManager = null;

        if (parentRegionManager != null)
        {
          actualRegionManager = parentRegionManager;
        }
        else
        {
          actualRegionManager = regionManagers.Single(x => x.Regions.Any(y => y.Name == regionName));
        }

        if (actualRegionManager.Regions.Count(x => x.Name == regionName) == 0)
        {
          throw new Exception($"RegionManager does not contains region {regionName}");
        }

        var view = Application.Current?.Dispatcher?.Invoke(() =>
        {
          return CreateView<TView, TViewModel>(regionName, viewModel, actualRegionManager, containsNestedRegion);
        });

        Views.Add(view);

        guid = view.Guid;

        SubscribeToChanges(view);

        var regionManager = view.RegionManager;

        if (regionManager != null)
        {
          regionManagers.Add(regionManager);
        }

        return regionManager;
      }
      else if (registredView is RegistredView<TView, TViewModel> view)
      {
        view.ViewModel = viewModel;

        guid = view.Guid;

        SubscribeToChanges(view);

        return view.RegionManager;
      }
      else
      {
        throw new NotImplementedException();
      }

    }

    #endregion RegisterView

    #region CreateView

    public RegistredView<TView, TViewModel> CreateView<TView, TViewModel>(
      string regionName,
      TViewModel viewModel,
      IRegionManager regionManager,
      bool initializeImmediately = false)
      where TViewModel : class, INotifyPropertyChanged, IActivable
      where TView : class, IView
    {
      var region = regionManager.Regions[regionName];

      return new RegistredView<TView, TViewModel>(region, viewFactory, viewModelsFactory, viewModel, initializeImmediately);
    }

    #endregion CreateView

    #region ActivateView

    public void ActivateView(Guid guid)
    {
      var view = Views.SingleOrDefault(x => x.Guid == guid);
      view?.Activate();
    }

    #endregion

    #region RefreshView

    public void RefreshView(Guid guid)
    {
      var view = Views.SingleOrDefault(x => x.Guid == guid);
      view?.Refresh();

    }

    #endregion

    #region DectivateView

    public void DectivateView(Guid guid)
    {
      var view = Views.SingleOrDefault(x => x.Guid == guid);
      view?.Deactivate();
    }

    #endregion 

    #region GoBack

    public void GoBack(Guid guid)
    {
      var view = Views.SingleOrDefault(x => x.Guid == guid);
      var before = navigationProvider.GetPrevious(view);
      before?.Activate();
    }

    #endregion

    #endregion Methods
  }
}