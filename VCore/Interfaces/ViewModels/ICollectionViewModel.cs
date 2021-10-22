using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Regions;

namespace VCore.WPF.Interfaces.ViewModels
{
  public interface ICollectionViewModel<TViewModel, TModel>
  {
    #region Methods

    public IQueryable<TModel> LoadQuery { get; }

    Task<ICollection<TViewModel>> GetViewModelsAsync(IQueryable<TModel> optionalQuery = null);

    void RecreateCollection();

    IRegionManager RegionManager { get; }

    LoadingStatus LoadingStatus { get; }

    #endregion Methods
  }
}