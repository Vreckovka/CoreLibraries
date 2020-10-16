using System.Collections.Generic;
using System.Linq;

namespace VCore.ItemsCollections.VirtualList
{
  public class ItemsGenerator<TViewModel> : IObjectGenerator<TViewModel> where TViewModel : class
  {
    #region Fields

    public readonly PagedRepository<TViewModel> _repository;

    #endregion 

    #region Constructors

    public ItemsGenerator(IEnumerable<TViewModel> source, int pageSize)
    {
      _repository = new PagedRepository<TViewModel>(source.ToArray(), pageSize);
    }

    #endregion 

    public TViewModel[] AllItems => _repository.source;
    public int Count => _repository.Count();


    #region Methods

    public TViewModel CreateObject(int index)
    {
      return _repository.GetAt(index);
    }

    #endregion
  }
}