using System;
using VCore.Standard.Common;

namespace VCore.Standard.Helpers
{
  public static class DisposableExtentions
  {
    public static TObject DisposeWith<TObject>(this TObject disposable, VDisposableObject disposableObject) where TObject :IDisposable
    {
      disposableObject.AddAutoDisposeObject(disposable);

      return disposable;
    }
  }
}