using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using VCore.Modularity.Navigation;
using VCore.Standard.Factories.ViewModels;
using VCore.Standard.Factories.Views;
using VCore.Standard.Modularity.Interfaces;

namespace VCore.Modularity.RegionProviders
{
  public class RegionProvider : IRegionProvider
  {
    

    #region Fields

    protected readonly IViewModelsFactory viewModelsFactory;
    private readonly INavigationProvider navigationProvider;
    private readonly IRegionManager globalRegionManager;
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
      this.globalRegionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
      this.viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
      this.viewModelsFactory = viewModelsFactory ?? throw new ArgumentNullException(nameof(viewModelsFactory));
      this.navigationProvider = navigationProvider ?? throw new ArgumentNullException(nameof(navigationProvider));

      RegionManager = regionManager;
    }

    #endregion Constructors

    #region Methods

    #region SubscribeToChanges

    private void SubscribeToChanges<TView, TViewModel>(RegistredView<TView, TViewModel> view)
      where TView : class, IView
      where TViewModel : class, INotifyPropertyChanged
    {
      var disposable = view.ViewWasActivated.Subscribe((x) =>
      {
        navigationProvider.Navigate(x);
      });

      ActivateSubscriptions.Add(view, disposable);
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
    where TViewModel : class, INotifyPropertyChanged
    {
      var registredView = Views.SingleOrDefault(x => x.ViewName == RegistredView<TView, TViewModel>.GetViewName(regionName));

      if (registredView == null)
      {
        var actualRegionManager = globalRegionManager;

        if (parentRegionManager != null)
        {
          actualRegionManager = parentRegionManager;
        }

        if (actualRegionManager.Regions.Count(x => x.Name == regionName) == 0)
        {
          IRegion region = new Region();

          region.Name = regionName;

          actualRegionManager.Regions.Add(region);
        }

        var view = Application.Current?.Dispatcher?.Invoke(() =>
        {
          return CreateView<TView, TViewModel>(regionName, viewModel, actualRegionManager, containsNestedRegion);
        });

        SubscribeToChanges(view);

        guid = view.Guid;

        return view.RegionManager;
      }
      else if (registredView is RegistredView<TView, TViewModel> view)
      {
        view.ViewModel = viewModel;

        guid = view.Guid;

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
      where TViewModel : class, INotifyPropertyChanged
      where TView : class, IView
    {
      var region = regionManager.Regions[regionName];

      return new RegistredView<TView, TViewModel>(region, viewFactory, viewModelsFactory, viewModel, initializeImmediately);
    }

    #endregion CreateView

    #region ActivateView

    public IRegionManager RegionManager { get; set; }

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