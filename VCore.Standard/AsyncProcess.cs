using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace VCore.Standard
{
  public class AsyncProcess<TResult>
  {
    private ReplaySubject<int> internalProcessedCountSubject = new ReplaySubject<int>(1);

    public Task<TResult> Process { get; set; }
    public int InternalProcessesCount { get; set; }

    #region ProcessedCount

    private int processedCount;
    public int ProcessedCount
    {
      get
      {
        return processedCount;
      }
      set
      {
        if (value != processedCount)
        {
          processedCount = value;
          internalProcessedCountSubject.OnNext(processedCount);
        }
      }
    }

    #endregion

    public IObservable<int> OnInternalProcessedCountChanged
    {
      get
      {
        return internalProcessedCountSubject.AsObservable();
      }
    }
  }
}