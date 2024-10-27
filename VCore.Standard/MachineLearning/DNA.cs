using System;
using System.Collections.Generic;

namespace TradingBroker.MachineLearning
{

  public class DNA<T>
  {
    protected readonly string[] keys;
    protected readonly Func<string, T> getRandomGene;
    protected readonly Func<T[], float> scoreFunction;
    protected readonly Func<double> getRandomDouble;

    public DNA(
      int size,
      Func<string, T> getRandomGene,
      Func<T[], float> scoreFunction,
      Func<double> getRandomDouble,
      string[] keys,
      bool shouldInitGenes = true)
    {
      this.getRandomGene = getRandomGene;
      this.scoreFunction = scoreFunction;
      this.getRandomDouble = getRandomDouble;
      this.keys = keys;

      Genes = new T[size];

      if (shouldInitGenes)
      {
        for (int i = 0; i < Genes.Length; i++)
        {
          if (keys != null)
          {
            Genes[i] = getRandomGene(keys[i]);
          }
          else
          {
            Genes[i] = getRandomGene(null);
          }
        }
      }
    }

    public T[] Genes { get; private set; }
    public float Fitness { get; set; } = float.MinValue;
    public double Score { get; set; }
    public float AdjustedFitness { get; set; }

    #region CalculateFitness

    public float CalculateFitness()
    {
      int exponentialRate = 3;

      var score = GetScore(Genes);
      double expScore = 0;

      if (score == float.MinValue)
      {
        Console.WriteLine("NASIEL SOM MIN");
        expScore = float.MinValue;
      }
      else
      {
        expScore = (Math.Pow(score, exponentialRate)) - 1 / (exponentialRate - 1);
      }

      
      Fitness = (float)expScore;

      Score = score;

      return Fitness;
    }

    #endregion
    
    protected virtual float GetScore(T[] genes)
    {
      return scoreFunction(genes);
    }

   

    #region Mutate

    public virtual void Mutate(float mutationRate)
    {
      for (int i = 0; i < Genes.Length; i++)
      {
        if (getRandomDouble() < mutationRate)
        {
          if (keys != null)
          {
            Genes[i] = getRandomGene(keys[i]);
          }
          else
          {
            Genes[i] = getRandomGene(null);
          }
        }
      }
    }

    #endregion

    private void GetRandomGene()
    {

    }
  }
}