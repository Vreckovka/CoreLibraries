using System;
using System.Collections.Generic;

namespace TradingBroker.MachineLearning
{
  public class DNA<T>
  {
    private readonly string[] keys;
    private readonly Func<string, T> getRandomGene;
    private readonly Func<T[], float> scoreFunction;
    private readonly Func<double> getRandomDouble;

    public DNA(
      int size,
      Func<string, T> getRandomGene,
      Func<T[], float> scoreFunction,
      Func<double> getRandomDouble,
      string[] keys,
      bool shouldInitGenes = true)
    {
      this.getRandomGene = getRandomGene ?? throw new ArgumentNullException(nameof(getRandomGene));
      this.scoreFunction = scoreFunction ?? throw new ArgumentNullException(nameof(scoreFunction));
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
    public float Fitness { get; private set; } = float.MinValue;
    public double Score { get; set; }

    #region CalculateFitness

    public float CalculateFitness(T[] genes)
    {
      int exponentialRate = 3;

      var score = scoreFunction(genes);
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

      if (Fitness == 0)
      {

      }


      Score = score;

      return Fitness;
    }

    #endregion

    #region Crossover

    public DNA<T> Crossover(DNA<T> otherParent)
    {
      DNA<T> child = new DNA<T>(Genes.Length, getRandomGene, scoreFunction, getRandomDouble, keys, false);


      for (int i = 0; i < Genes.Length; i++)
      {
        var rnd = getRandomDouble();

        if (rnd < 0.50)
        {
          child.Genes[i] = Genes[i];
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

    #region Mutate

    public void Mutate(float mutationRate)
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
  }
}