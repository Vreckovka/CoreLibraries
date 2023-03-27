using System;
using System.Windows.Input;
using Prism.Regions;
using VCore.Standard;
using VCore.Standard.Modularity.Interfaces;
using VCore.WPF.Misc;
using VCore.WPF.Modularity.RegionProviders;
using VCore.WPF.ViewModels.Navigation;

namespace VCore.WPF.ViewModels
{
  public abstract class RegionViewModel : ViewModel
  {
    public Guid Guid { get; protected set; }

    public virtual bool ContainsNestedRegions { get; }

    public abstract string RegionName { get; protected set; }

    public IRegionManager RegionManager { get; protected set; }

   
  }

  public abstract class RegionViewModel<TView> : RegionViewModel, INavigationItem, IRegionViewModel where TView : class, IView
  {
    #region Fields

    protected readonly IRegionProvider regionProvider;

    #endregion Fields

    #region Constructors

    public RegionViewModel(IRegionProvider regionProvider)
    {
      this.regionProvider = regionProvider ?? throw new ArgumentNullException(nameof(regionProvider));
      Header = this.ToString();
    }

    #endregion Constructors

    #region Properties

    public virtual string Header { get; }

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
            OnActivation(!wasActivated);

            if (!wasActivated)
            {
              wasActivated = true;
            }
          }
          else
          {
            OnDeactived();
          }

          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #endregion

    #region BackCommand

    private ActionCommand backCommand;

    public ICommand BackCommand
    {
      get
      {
        if (backCommand == null)
        {
          backCommand = new ActionCommand(OnBackCommand);
        }

        return backCommand;
      }
    }

    protected virtual void OnBackCommand()
    {
      regionProvider.GoBack(Guid);
    }

    #endregion BackCommand

    #region Methods

    #region OnActivation

    public virtual void OnActivation(
      bool firstActivation)
    {
      if (firstActivation)
      {
        RegionManager = regionProvider.RegisterView<TView, RegionViewModel<TView>>(RegionName, this, ContainsNestedRegions, out var newGuid, RegionManager);

        Guid = newGuid;
      }

    }

    #endregion OnActivation

    #region OnDeactived

    public virtual void OnDeactived()
    {

    }

    #endregion

    #endregion


  }
}