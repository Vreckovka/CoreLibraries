using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VCore.Standard.Common
{
  public abstract class VBindableBase : VDisposableObject, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
      OnPropertyChanged(propertyName);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}