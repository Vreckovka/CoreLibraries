using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace TradingBroker.MachineLearning
{

  public class Gene<T>
  {
    public string Key { get; set; }
    public T Value { get; set; }
  }

  public class GeneticAlgorithmManager<TData>
  {
    protected CancellationTokenSource cancellationTokenSource;
    protected int threadCount;
    protected List<ThreadingGeneticAlgorithm<TData>> algorithms = new List<ThreadingGeneticAlgorithm<TData>>();
    protected int populationSize;
    protected readonly string[] keys;
    protected readonly int geneLength;


    private Random random = new Random();
    private Random random1 = new Random();

    private volatile List<DNA<TData>> totalGenerations = new List<DNA<TData>>();

    private int generationCreatedCount = 0;


    private readonly Func<TData[], float> getScore;
    private readonly Func<string, TData> getRandomGene;


    private SemaphoreSlim mutex = new SemaphoreSlim(1, 1);


    #region Constructors

    public GeneticAlgorithmManager(
      int threadCount,
      int populationSize,
      int geneLength,
      string[] keys = null)
    {
      this.threadCount = threadCount;
      this.populationSize = populationSize;
      this.geneLength = geneLength;
      this.keys = keys;
    }

    public GeneticAlgorithmManager(
      int threadCount,
      int populationSize,
      int geneLength,
      Func<TData[], float> getScore,
      Func<string, TData> getRandomGene,
      string[] keys = null) : this(threadCount, populationSize, geneLength, keys)
    {
      this.getScore = getScore ?? throw new ArgumentNullException(nameof(getScore));
      this.getRandomGene = getRandomGene ?? throw new ArgumentNullException(nameof(getRandomGene));
    }

    #endregion

    public int Iteration { get; set; }

    protected ReplaySubject<DNA<TData>> newPopulationsCreatedSubject = new ReplaySubject<DNA<TData>>();

    public IObservable<DNA<TData>> OnNewGenerationCreated
    {
      get
      {
        return newPopulationsCreatedSubject.AsObservable();
      }
    }

    #region IsRunning

    private bool isRunning;

    public bool IsRunning
    {
      get { return isRunning; }
      set
      {
        isRunning = value;

        if (!isRunning)
        {
          cancellationTokenSource.Cancel();
        }
      }
    }

    #endregion

    #region Methods

    #region Run

    public virtual void Run(CancellationToken cancellationToken)
    {
      List<Thread> threads = new List<Thread>();

      IsRunning = true;
     
      for (int i = 0; i < threadCount; i++)
      {

        var newThread = new Thread(() =>
        {
          var geneticAlgorithm = new ThreadingGeneticAlgorithm<TData>(populationSize, geneLength, GetRandomNext, GetRandomDouble, getRandomGene, getScore, keys, cancellationToken);

          algorithms.Add(geneticAlgorithm);

          geneticAlgorithm.MaxGenerationCount = 1000;

          geneticAlgorithm.ElitismCount = 5;

          geneticAlgorithm.generationCreated.Subscribe(GenerationGenerated);

          geneticAlgorithm.Run();
        });

        newThread.Name = i.ToString();

        threads.Add(newThread);
        newThread.Start();
      }


      foreach (var thread in threads)
      {
        thread.Join();
      }
    }

    #endregion

    #region GetRandomNext

    public int GetRandomNext(int a, int b)
    {
      lock (this)
      {
        return random1.Next(a, b);
      }
    }

    #endregion

    #region GetRandomDouble

    public double GetRandomDouble()
    {
      lock (this)
      {
        return random1.NextDouble();
      }
    }

    #endregion

    #region GenerationGenerated

    protected void GenerationGenerated(List<DNA<TData>> generation)
    {
      lock (this)
      {
        totalGenerations.AddRange(generation);

        generationCreatedCount++;


        if (generationCreatedCount == threadCount)
        {

          generationCreatedCount = 0;

          CreateNewPopulations();
        }
      }
    }

    #endregion

    #region CreateNewPopulations

    private void CreateNewPopulations()
    {
      try
      {
        Iteration++;

        mutex.Wait();

        List<List<DNA<TData>>> poulations = new List<List<DNA<TData>>>();

        for (int i = 0; i < threadCount; i++)
        {
          poulations.Add(new List<DNA<TData>>());
        }


        for (int i = 0; i < totalGenerations.Count; i++)
        {
          List<DNA<TData>> selectedPopulation = null;

          while (selectedPopulation == null)
          {
            var randomPopulation = random.Next(0, threadCount);

            if (poulations[randomPopulation].Count < populationSize)
            {
              selectedPopulation = poulations[randomPopulation];
            }
            else if (poulations.Sum(x => x.Count) == totalGenerations.Count)
            {
              return;
            }
          }


          selectedPopulation.Add(totalGenerations[i]);
        }

        var best = totalGenerations.OrderByDescending(x => x.Fitness).First();

        newPopulationsCreatedSubject.OnNext(best);

        for (int i = 0; i < algorithms.Count; i++)
        {
          var algorith = algorithms[i];

          algorith.Population = poulations[i];
          algorith.semaphor.Release();
        }

        totalGenerations.Clear();
      }
      catch (Exception ex)
      {
        throw;
      }
      finally
      {
        mutex.Release();
      }
    }

    #endregion 

    #endregion
  }
}