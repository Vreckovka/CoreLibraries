using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using VCore.Standard;

namespace VCore.WPF.ViewModels
{
  public class VDispatcherTimer : VTimer
  {
    public VDispatcherTimer()
    {
    
    }

    public VDispatcherTimer(int dueTime)
    {
      DueTime = dueTime;
    }

    private Stopwatch stopwatchReloadVirtulizedPlaylist;
    private SerialDisposable serialDisposable = new SerialDisposable();
    private List<Action> actions = new List<Action>();

    private void Request()
    {
      serialDisposable.Disposable = Observable.Timer(TimeSpan.FromMilliseconds(DueTime)).Subscribe(async (x) =>
      {
        try
        {
          await semaphoreSlim.WaitAsync();

          stopwatchReloadVirtulizedPlaylist = null;
          if (actions.Count > 0)
             await VSynchronizationContext.InvokeOnDispatcherAsync(() => actions.ForEach(x => x.Invoke()));
          actions.Clear();
        }
        finally
        {
          semaphoreSlim.Release();
        }
      });

      if (stopwatchReloadVirtulizedPlaylist == null || stopwatchReloadVirtulizedPlaylist.ElapsedMilliseconds > DueTime)
      {
        if (actions.Count > 0)
          VSynchronizationContext.InvokeOnDispatcher(() => actions.ForEach(x => x.Invoke()));

        actions.Clear();

        serialDisposable.Disposable?.Dispose();
        stopwatchReloadVirtulizedPlaylist = new Stopwatch();
        stopwatchReloadVirtulizedPlaylist?.Start();
      }
    }

    private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
    public async void RequestMethodCallOnDispatcher(Action action)
    {

      try
      {
        await semaphoreSlim.WaitAsync();

        actions.Add(action);

        Request();
      }
      finally
      {
        semaphoreSlim.Release();
      }
    }



    public override void Dispose()
    {
      actions.Clear();

      base.Dispose();
    }
  }
}