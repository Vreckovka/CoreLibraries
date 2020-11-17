using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;
using VCore;

namespace TradingBroker.MachineLearning
{
  public class ThreadingGeneticAlgorithm<T> : GeneticAlgorithm<T>
  {
    private readonly CancellationToken? cancellationToken;

    #region Constructors

    public ThreadingGeneticAlgorithm(int populationSize, int geneSize, Func<int, int, int> getRandomNext,
      Func<double> getRandomDouble,
      Func<string,T> getRandomGene,
      Func<T[], float> scoreFunction,
      string[] keys,
      CancellationToken? cancellationToken = null,
      float mutationRate = (float)0.01) : base(populationSize, geneSize, getRandomNext, getRandomDouble, getRandomGene, scoreFunction,keys, mutationRate)
    {
      this.cancellationToken = cancellationToken;
    }

    #endregion


    public Subject<List<DNA<T>>> generationCreated = new Subject<List<DNA<T>>>();
    public SemaphoreSlim semaphor { get; } = new SemaphoreSlim(1, 1);

    #region Run

    public override void Run()
    {
      int doneIteration = 0;

      while (IsRunning)
      {

        semaphor.Wait();

        if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
        {
          return;
        }

        CreateNewGeneration();

        doneIteration++;

        generationCreated.OnNext(Population);

        if (MaxGenerationCount == doneIteration)
        {
          generationCreated.OnCompleted();
          break;
        }
      }
    }

    #endregion
  }
}