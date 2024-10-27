using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using TradingBroker.MachineLearning;

namespace TradingBroker.MachineLearning
{

  public class GeneticAlgorithm<T> : BaseGeneticAlgorithm<T, DNA<T>>
  {
    public GeneticAlgorithm(int populationSize,
     int geneSize,
     Func<string, T> getRandomGene,
     Func<T[], float> scoreFunction,
     string[] keys,
     Func<int, int, int> getRandomNext = null,
     Func<double> getRandomDouble = null,
     float mutationRate = 0.01f) :
      base(populationSize, geneSize, getRandomGene, scoreFunction, keys, getRandomNext, getRandomDouble, mutationRate)
    {

    }

    protected override DNA<T> GetNewDNA(bool init)
    {
      return new DNA<T>(geneSize, getRandomGene, scoreFunction, getRandomDouble, keys, init);
    }
  }

  public abstract class BaseGeneticAlgorithm<T, TDNA>
    where TDNA : DNA<T>
  {
    #region Fields

    protected Random random = new Random();
    protected readonly Func<int, int, int> getRandomNext;
    protected readonly Func<double> getRandomDouble;
    protected Func<string, T> getRandomGene;
    protected Func<T[], float> scoreFunction;
    protected readonly string[] keys;

    protected readonly int populationSize;
    protected readonly int geneSize;


    protected float fitnessSum;

    protected float bestFitness;
    protected T[] bestGenes;
    protected double bestScore;

    private Subject<KeyValuePair<T[], double>> generationGenerated = new Subject<KeyValuePair<T[], double>>();

    int exponentialRate = 3;

    #endregion

    #region Contructors

    public BaseGeneticAlgorithm(int populationSize,
      int geneSize,
      Func<string, T> getRandomGene,
      Func<T[], float> scoreFunction,
      string[] keys,
      Func<int, int, int> getRandomNext = null,
      Func<double> getRandomDouble = null,
      float mutationRate = 0.01f)
    {
      Generation = 1;

      MutationRate = mutationRate;

      this.populationSize = populationSize;
      this.geneSize = geneSize;
      this.getRandomNext = getRandomNext ?? GetRandomNext;
      this.getRandomDouble = getRandomDouble ?? GetRandomDouble;
      this.getRandomGene = getRandomGene;
      this.scoreFunction = scoreFunction;
      this.keys = keys;

      Population = new List<TDNA>(populationSize);
      newPopulation = new List<TDNA>(populationSize);

      bestGenes = new T[geneSize];
    }

    #endregion

    #region Properties

    public List<TDNA> Population { get; set; }
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

    protected abstract TDNA GetNewDNA(bool init);

    public virtual void SeedGeneration()
    {
      if (Population.Count == 0)
      {
        for (int i = 0; i < populationSize; i++)
        {
          Population.Add(GetNewDNA(true));
        }
      }
    }

    #region CreateNewGeneration

    private List<TDNA> newPopulation;

    public virtual void CreateNewGeneration()
    {
      SeedGeneration();

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
          TDNA parentA = ChooseParent();
          TDNA parentB = ChooseParent();

          TDNA child = Crossover(parentA,parentB);

          child.Mutate(MutationRate);

          newPopulation.Add(child);
        }
      }

      List<TDNA> tmp = Population;
      Population = newPopulation;
      newPopulation = tmp;

      Generation++;
    }

    #endregion

    #region Crossover

    public TDNA Crossover(TDNA parent, TDNA otherParent)
    {
      var parentGenes = parent.Genes;

      TDNA child = GetNewDNA(false);

      for (int i = 0; i < parentGenes.Length; i++)
      {
        var rnd = getRandomDouble();

        if (rnd < 0.50)
        {
          child.Genes[i] = parentGenes[i];
        }
        else if (rnd > 0.50)
        {
          child.Genes[i] = otherParent.Genes[i];
        }
        else
        {
          i--;
        }
      }

      return child;
    }

    #endregion

    #region CalculateFitness

    protected virtual void CalculateFitness()
    {
      fitnessSum = 0;
      var best = Population[0];
      float minFitness = float.MaxValue;

      // Find minimum fitness
      foreach (var dna in Population)
      {
        dna.CalculateFitness();

        if (dna.Fitness < minFitness)
        {
          minFitness = dna.Fitness;
        }
      }

      // Adjust fitness values if there are negative fitnesses
      if (minFitness < 0)
      {
        foreach (var dna in Population)
        {
          dna.Fitness -= minFitness;
        }
      }

      foreach (var dna in Population)
      {
        fitnessSum += dna.Fitness;

        if (dna.Fitness > best.Fitness)
        {
          best = dna;
        }
      }

      bestFitness = best.Fitness;
      bestScore = best.Score;
      best.Genes.CopyTo(bestGenes, 0);
    }

    #endregion

    #region ChooseParent

    protected virtual TDNA ChooseParent()
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

    public int CompareDNA(TDNA a, TDNA b)
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

    #region GetRandomDouble

    protected double GetRandomDouble()
    {
      return random.NextDouble();
    }

    #endregion

    #region GetRandomNext

    protected int GetRandomNext(int a, int b)
    {
      lock (this)
      {
        return random.Next(a, b);
      }
    }

    #endregion
   

    #endregion
  }
}
