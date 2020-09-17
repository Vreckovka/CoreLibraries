using System;

namespace VCore.Modularity.Events
{
  public class ItemChanged
  {
    public object Item { get; set; }
    public Changed Changed { get; set; }
  }

  public class ItemChanged<TModel>
  {
    public TModel Item { get; }
    public Changed Changed { get; }

    public ItemChanged(TModel item, Changed changed)
    {
      this.Item = item ?? throw new ArgumentNullException(nameof(Item));

      Changed = changed;
    }
  }
}