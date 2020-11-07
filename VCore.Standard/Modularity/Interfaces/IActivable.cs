using System.ComponentModel;

namespace VCore.Standard.Modularity.Interfaces
{
  public interface IActivable : INotifyPropertyChanged
  {
    bool IsActive { get; set; }
  }
}
