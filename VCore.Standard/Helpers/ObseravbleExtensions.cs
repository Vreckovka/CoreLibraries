using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace VCore.Standard.Helpers
{
  public static class ObseravbleExtensions
  {
    public static IObservable<T> StepInterval<T>(this IObservable<T> source, TimeSpan minDelay)
    {
      return source.Select(x =>
        Observable.Empty<T>()
          .Delay(minDelay)
          .StartWith(x)
      ).Concat();
    }

    public static IObservable<IList<T>> BufferUntilInactive<T>(this IObservable<T> stream, TimeSpan delay)
    {
      var closes = stream.Throttle(delay);
      return stream.Window(() => closes).SelectMany(window => window.ToList());
    }
  }
}