using System;
using VCore.Standard.Common;

namespace VCore.Standard.Helpers
{
  public static class DisposableExtentions
  {
    public static void DisposeWith(this IDisposable disposable, VDisposableObject disposableObject)
    {
      disposableObject.AddAutoDisposeObject(disposable);
    }
  }
}