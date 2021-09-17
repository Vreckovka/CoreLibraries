using System;
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
  }
}