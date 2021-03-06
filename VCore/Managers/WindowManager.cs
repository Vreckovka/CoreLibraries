using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using VCore.Standard;
using VCore.Standard.Modularity.Interfaces;

namespace VCore.WPF.Managers
{
  public class WindowManager : IWindowManager
  {
    #region ShowYesNoPrompt

    public MessageBoxResult ShowYesNoPrompt(string text, string header = "")
    {
      return MessageBox.Show(header, text, MessageBoxButton.YesNo, MessageBoxImage.Warning);
    }

    #endregion

    #region ShowPrompt

    public MessageBoxResult ShowPrompt(string text, string header = "")
    {
      return MessageBox.Show(header, text, MessageBoxButton.OK);
    }

    #endregion

    #region ShowPrompt

    public void ShowPrompt<TView>(ViewModel viewModel) where TView : IView, new()
    {
      var window = new Window();

      window.Content = new TView();

      window.DataContext = viewModel;

      window.SizeToContent = SizeToContent.WidthAndHeight;

      window.ShowDialog();
    }

    #endregion
  }
}
