using System;
using System.Windows;
using VCore.Standard;
using VCore.Standard.Modularity.Interfaces;

namespace VCore.WPF.Managers
{
  public interface IWindowManager
  {
    MessageBoxResult ShowYesNoPrompt(string text, string header = "");
    MessageBoxResult ShowPrompt(string text, string header = "");
    void ShowPrompt<TView>(ViewModel viewModel) where TView : IView, new();
    void ShowErrorPrompt(Exception ex);
  }
}