using Prism.Regions;
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using VCore.Helpers;
using VCore.Standard.Factories.ViewModels;
using VCore.Standard.Factories.Views;
using VCore.Standard.Modularity.Interfaces;
using VCore.ViewModels;

namespace VCore.Modularity.RegionProviders
{
  public class RegistredView<TView, TViewModel> : IDisposable, IRegistredView
    where TView : class, IView
    where TViewModel : class, INotifyPropertyChanged, IActivable
  {

    #region Fields

    private readonly IViewFactory viewFactory;
    private readonly IViewModelsFactory viewModelsFactory;

    #endregion Fields

    #region Constructors

    public RegistredView(
      IRegion region,
      IViewFactory viewFactory,
      IViewModelsFactory viewModelsFactory,
      TViewModel viewModel = null
    ) : this(region, viewFactory, viewModelsFactory, viewModel, false)
    {
    }

    public RegistredView(
      IRegion region,
      IViewFactory viewFactory,
      IViewModelsFactory viewModelsFactory,
      TViewModel viewModel = null,
      bool initializeImmediately = false
    )
    {
      this.viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
      this.viewModelsFactory = viewModelsFactory ?? throw new ArgumentNullException(nameof(viewModelsFactory));

      Guid = Guid.NewGuid();

      ViewModel = viewModel;
      ViewName = GetViewName(region.Name);
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

          if (viewModel != null)
          {
            //viewActivationDisposable.Disposable = viewModel.ObservePropertyChange(x => x.IsActive).Subscribe(x =>
            //{
            //  if (x)
            //    viewWasActivatedSubject.OnNext(this);
            //  else
            //    viewWasDeactivatedSubject.OnNext(this);
            //});
          }

          if (View != null)
            View.DataContext = viewModel;
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
        return viewFactory.Create<TView>();
      });
    }

    #endregion Create

    #region GetViewName

    public static string GetViewName(string regionName)
    {
      return typeof(TView).Name + "_" + typeof(TViewModel).Name + "_" + regionName;
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