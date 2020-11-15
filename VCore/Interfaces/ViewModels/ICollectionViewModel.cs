using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Regions;

namespace VCore.Interfaces.ViewModels
{
  public interface ICollectionViewModel<TViewModel, TModel>
  {
    #region Methods

    Task<ICollection<TViewModel>> GetViewModelsAsync(IQueryable<TModel> optionalQuery = null);

    void RecreateCollection();

    IRegionManager RegionManager { get; set; }

    #endregion Methods
  }
}