﻿using System;
using System.Collections;
using System.Windows.Controls;
using Prism.Events;

namespace VCore.Controls
{
  public class PlayableWrapPanel : ItemsControl
  {

    public PlayableWrapPanel()
    {

    }

    protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
    {
      base.OnItemsSourceChanged(oldValue, newValue);
    }
  }
}
