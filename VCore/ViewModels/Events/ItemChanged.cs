﻿namespace VCore.WPF.ViewModels.Events
{
  public class ItemChanged
  {
    public object Item { get; set; }
    public ChangeEvent ChangeEvent { get; set; }
  }
}