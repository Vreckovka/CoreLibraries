using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using TradingBroker.MachineLearning;

namespace TradingBroker.MachineLearning
{
  public class GeneticAlgorithm<T>
  {
    #region Fields

    private readonly Func<int, int, int> getRandomNext;
    private readonly Func<double> getRandomDouble;
    protected Func<string, T> getRandomGene;
    protected Func<T[], float> scoreFunction;
    private readonly string[] keys;

    private readonly int populationSize;
    private readonly int geneSize;
  

    private float fitnessSum;

    private float bestFitness;
    private T[] bestGenes;
    private double bestScore;

    private Subject<KeyValuePair<T[], double>> generationGenerated = new Subject<KeyValuePair<T[], double>>();

    int exponentialRate = 3;

    #endregion

    #region Contructors

    public GeneticAlgorithm(int populationSize,
      int geneSize,
      Func<int, int, int> getRandomNext,
      Func<double> getRandomDouble,
      Func<string,T> getRandomGene,
      Func<T[], float> scoreFunction,
      string[] keys,
      float mutationRate = 0.01f)
    {
      Generation = 1;

      MutationRate = mutationRate;

      this.populationSize = populationSize;
      this.geneSize = geneSize;
      this.getRandomNext = getRandomNext;
      this.getRandomDouble = getRandomDouble;
      this.getRandomGene = getRandomGene ?? throw new ArgumentNullException(nameof(getRandomGene));
      this.scoreFunction = scoreFunction ?? throw new ArgumentNullException(nameof(scoreFunction));
      this.keys = keys;

      Population = new List<DNA<T>>(populationSize);
      newPopulation = new List<DNA<T>>(populationSize);

      bestGenes = new T[geneSize];
    }

    #endregion

    #region Properties

    public List<DNA<T>> Population { get; set; }
    public int Generation { get; private set; }
    public float MutationRate { get; set; } = (float)0.01;
    public int? MaxGenerationCount { get; set; }
    public int ElitismCount { get; set; } = 5;

    public bool IsRunning { get; set; } = true;

    #region OnGenerationGenerated

    public IObservable<KeyValuePair<T[], double>> OnGenerationGenerated
    {
      get { return generationGenerated.AsObservable(); }
    }

    #endregion

    #endregion

    #region Methods

    #region CreateNewGeneration

    private List<DNA<T>> newPopulation;

    protected void CreateNewGeneration()
    {
      if (Population.Count == 0)
      {
        for (int i = 0; i < populationSize; i++)
        {
          Population.Add(new DNA<T>(geneSize, getRandomGene, scoreFunction, getRandomDouble,keys, true));
        }
      }

      newPopulation.Clear();

      CalculateFitness();

      Population.Sort(CompareDNA);


      for (int i = 0; i < Population.Count; i++)
      {
        if (i < ElitismCount)
        {
          newPopulation.Add(Population[i]);
        }
        else
        {
          DNA<T> parentA = ChooseParent();
          DNA<T> parentB = ChooseParent();

          DNA<T> child = parentA.Crossover(parentB);

          child.Mutate(MutationRate);

          newPopulation.Add(child);
        }
      }

      List<DNA<T>> tmp = Population;
      Population = newPopulation;
      newPopulation = tmp;

      Generation++;
    }

    #endregion

    #region CalculateFitness

    private void CalculateFitness()
    {
      fitnessSum = 0;
      var best = Population[0];

      for (int i = 0; i < Population.Count; i++)
      {
        var dna = Population[i];

        fitnessSum += dna.CalculateFitness(dna.Genes);

        if (Population[i].Fitness > best.Fitness)
        {
          best = Population[i];
        }
      }

      bestFitness = best.Fitness;
      bestScore = best.Score;

      best.Genes.CopyTo(bestGenes, 0);
    }

    #endregion

    #region ChooseParent

    protected DNA<T> ChooseParent()
    {
      double randomNumber = getRandomDouble() * fitnessSum;

      for (int i = 0; i < Population.Count; i++)
      {
        if (randomNumber < Population[i].Fitness)
        {
          return Population[i];
        }
        else
        {
          randomNumber -= Population[i].Fitness;
        }
      }


      return Population[getRandomNext(0, Population.Count)];
    }

    #endregion

    #region Run

    public virtual void Run()
    {
      int doneIteration = 0;

      while (IsRunning)
      {
        CreateNewGeneration();

        doneIteration++;

        Console.WriteLine("Generation calculated: " + doneIteration);

        if (MaxGenerationCount == doneIteration)
        {
          generationGenerated.OnCompleted();
          break;
        }

      }
    }

    #endregion

    #region CompareDNA

    public int CompareDNA(DNA<T> a, DNA<T> b)
    {
      if (a.Fitness > b.Fitness)
      {
        return -1;
      }
      else if (a.Fitness < b.Fitness)
      {
        return 1;
      }
      else
      {
        return 0;
      }
    }

    #endregion

   
    
    #endregion
  }
}
