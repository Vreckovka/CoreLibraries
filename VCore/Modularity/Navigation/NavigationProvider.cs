using Prism.Regions;
using System.Collections.Generic;
using VCore.Modularity.RegionProviders;

namespace VCore.Modularity.Navigation
{
  public interface INavigationProvider
  {
    #region Properties

    Dictionary<IRegion, NavigationSet> NavigationItems { get; }

    #endregion Properties

    #region Methods

    IRegistredView GetNext(IRegistredView registredView);

    IRegistredView GetPrevious(IRegistredView registredView);

    void Navigate(IRegistredView registredView, IRegion region = null);

    #endregion Methods
  }

  public class NavigationProvider : INavigationProvider
  {
    #region Constructors

    public NavigationProvider()
    {
    }

    #endregion Constructors

    #region Properties

    public Dictionary<IRegion, NavigationSet> NavigationItems { get; } = new Dictionary<IRegion, NavigationSet>();

    #endregion Properties

    #region Methods

    #region GetNext

    public IRegistredView GetNext(IRegistredView registredView)
    {
      if (NavigationItems.TryGetValue(registredView.Region, out var navigationItems))
      {
        return navigationItems.GetNext();
      }

      return null;
    }

    #endregion

    #region GetPrevious

    public IRegistredView GetPrevious(IRegistredView registredView)
    {
      if (NavigationItems.TryGetValue(registredView.Region, out var navigationItems))
      {
        return navigationItems.GetPrevious();
      }

      return null;
    }

    #endregion

    #region Navigate

    public void Navigate(IRegistredView registredView, IRegion region = null)
    {
      var requestedRegion = registredView.Region;

      if (region != null)
      {
        requestedRegion = region;

        registredView.Region = region;
      }

      if (NavigationItems.TryGetValue(requestedRegion, out var navigationItems))
      {
        if (navigationItems.Actual.Value != registredView)
        {
          navigationItems.Actual.Value.DeactivateDataContext();
          navigationItems.Add(registredView);
        }
        else
          return;
      }
      else
      {
        var list = new NavigationSet();
        list.Add(registredView);

        NavigationItems.Add(registredView.Region, list);
      }

      registredView.Activate();
    }

    #endregion

    #endregion 
  }
}