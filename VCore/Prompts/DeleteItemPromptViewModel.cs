using System.Windows;

namespace VCore.WPF.Prompts
{
  public class DeleteItemPromptViewModel : GenericPromptViewModel
  {
    public DeleteItemPromptViewModel()
    {
      CanExecuteOkCommand = () => { return true; };
      CancelVisibility = Visibility.Visible;
    }

    public string ItemName { get; set; }

    public string AfterText { get; set; }
  }
}