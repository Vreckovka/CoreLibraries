using System;
using VCore.Common;

namespace VCore.Helpers
{
  public static class DisposableExtentions
  {
    public static void DisposeWith(this IDisposable disposable, VDisposableObject disposableObject)
    {
      disposableObject.AddAutoDisposeObject(disposable);
    }
  }
}