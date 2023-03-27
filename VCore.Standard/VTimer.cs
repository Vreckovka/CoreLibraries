using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using VCore.Standard.Common;

namespace VCore.Standard
{
  public class VTimer : VDisposableObject
  {
    public VTimer()
    {
        
    }

    public VTimer(int dueTime) : base()
    {
      DueTime = dueTime;
    }

    public int DueTime { get; set; } = 1500;

    private Stopwatch stopwatchReloadVirtulizedPlaylist;
    private object batton = new object();
    private SerialDisposable serialDisposable = new SerialDisposable();

    public void RequestMethodCall(Action action)
    {
      lock (batton)
      {
        serialDisposable.Disposable = Observable.Timer(TimeSpan.FromMilliseconds(DueTime)).Subscribe((x) =>
        {
          stopwatchReloadVirtulizedPlaylist = null;
          action.Invoke();
        });

        if (stopwatchReloadVirtulizedPlaylist == null || stopwatchReloadVirtulizedPlaylist.ElapsedMilliseconds > DueTime)
        {
          action.Invoke();

          serialDisposable.Disposable?.Dispose();
          stopwatchReloadVirtulizedPlaylist = new Stopwatch();
          stopwatchReloadVirtulizedPlaylist?.Start();
        }
      }
    }

    public override void Dispose()
    {
      base.Dispose();

      serialDisposable?.Dispose();
      stopwatchReloadVirtulizedPlaylist?.Stop();
    }
  }
}