using System;
using System.Collections.Generic;
using System.Text;
using VCore.Standard.Modularity.Interfaces;
using VCore.WPF.ViewModels.Prompt;

namespace VCore.WPF.Prompts
{
  public class GenericPromptViewModel : PromptViewModel
  {
    public GenericPromptViewModel()
    {
      CanExecuteOkCommand = () => { return true; };
    }

    public string Text { get; set; }
  }
}
