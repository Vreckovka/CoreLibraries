using System;
using System.Reactive.Disposables;

namespace VCore.Standard.Common
{
  [Serializable]
  public abstract class VDisposableObject : IDisposable
  {
    private CompositeDisposable autoDisposeObjects = null;

    ~VDisposableObject()
    {
      Dispose(false);
    }

    public bool IsDisposed { get; private set; }

    #region AddAutoDisposeObject

    protected internal void AddAutoDisposeObject(IDisposable disposableObject)
    {
      lock (this)
      {
        if (autoDisposeObjects == null)
          autoDisposeObjects = new CompositeDisposable();

        autoDisposeObjects.Add(disposableObject); 
      }
    }

    #endregion

    #region Dispose
    
    public virtual void Dispose()
    {
      Dispose(true);
    }

    public virtual void Dispose(bool disposing)
    {
      lock (this)
      {
        if (!disposing || this.IsDisposed)
        {
          return;
        }

        this.IsDisposed = true;

        if (autoDisposeObjects != null)
        {
          foreach (var disposable in autoDisposeObjects)
          {
            disposable?.Dispose();
          }
        }

        GC.SuppressFinalize(this);
      }
    }

    #endregion
  }
}