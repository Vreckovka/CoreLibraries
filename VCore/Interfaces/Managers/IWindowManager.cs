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
    PromptResult OkCancel(string text, string header = "", Visibility cancelVisibility = Visibility.Collapsed);
    PromptResult ShowDeletePrompt(string itemName, string header = "Delete", string beforeText = "Do you really want to delete item ", string afterText = " ?");
    PromptResult ShowQuestionPrompt<TView, TViewModel>(TViewModel viewModel)
      where TView : IView, new()
      where TViewModel : BasePromptViewModel;

    public PromptResult ShowQuestionPrompt(
      string beforeText = "",
      string header = "",
      string itemName = "",
      string afterText = "");

    /// <summary>
    /// You can use PromptViewModel
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    /// <param name="viewModel"></param>
    void ShowPrompt<TView>(ViewModel viewModel, double width = 0, double height = 0, bool showOverlay = true) where TView : IView, new();
    void ShowErrorPrompt(Exception ex);
    void ShowErrorPrompt(string message);
    Window GetWindowView<TView>(object dataContext, bool setOwner = true) where TView : IView, new();
  }
}