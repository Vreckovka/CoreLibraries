using System;

namespace VCore.Modularity.Events
{
  public interface IItemChanged
  {
    public Changed Changed { get; set; }
  }

  public interface IItemChanged<TModel> : IItemChanged
  {
    public TModel Item { get; }
  }


  public class ItemChanged : IItemChanged
  {
    public object Item { get; set; }

    public Changed Changed { get; set; }
  }

  public class ItemChanged<TModel> : IItemChanged<TModel>
  {
    public TModel Item { get; }
    public Changed Changed { get; set; }

    public ItemChanged(TModel item)
    {
      this.Item = item ?? throw new ArgumentNullException(nameof(Item));
    }
  }
}