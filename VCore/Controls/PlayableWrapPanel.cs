using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace VCore.Controls
{
  [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(PlayableWrapPanelItem))]
  public class PlayableWrapPanel : ItemsControl
  {

    public PlayableWrapPanel()
    {

    }

    protected override DependencyObject GetContainerForItemOverride()
    {
      return new PlayableWrapPanelItem();
    }

    protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
    {
      base.OnItemsSourceChanged(oldValue, newValue);
    }
  }
}
