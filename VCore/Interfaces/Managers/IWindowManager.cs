using System;
using System.Windows;
using VCore.Standard;
using VCore.Standard.Modularity.Interfaces;
using VCore.WPF.Prompts;
using VCore.WPF.ViewModels.Prompt;

namespace VCore.WPF.Interfaces.Managers
{
  public interface IWindowManager
  {
    PromptResult ShowYesNoPrompt(string text, string header = "");
    PromptResult ShowDeletePrompt(string itemName, string header = "Delete", string beforeText = "Do you really want to delete item ", string afterText = " ?");
    PromptResult ShowQuestionPrompt<TView, TViewModel>(TViewModel viewModel)
      where TView : IView, new()
      where TViewModel : BasePromptViewModel;
 

    void ShowPrompt<TView>(ViewModel viewModel) where TView : IView, new();
    void ShowErrorPrompt(Exception ex);
    void ShowErrorPrompt(string message);
    Window GetWindowView<TView>(object dataContext, bool setOwner = true) where TView : IView, new();
  }
}