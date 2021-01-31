using System;
using System.Threading;
using VCore;

namespace TradingBroker.MachineLearning
{
  public class TestGeneticAlgorithm
  {
    private string phrase = "Be or not to Be or be a bee";
    string chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^& ,";

    private Random Random = new Random();
    private GeneticAlgorithmManager<char> algorithm;

    public TestGeneticAlgorithm()
    {
      algorithm = new GeneticAlgorithmManager<char>(3, 200, phrase.Length, GetScore, GetGene);
    }

    public void Run()
    {
      algorithm.OnNewGenerationCreated.Subscribe((x) =>
      {
        Console.WriteLine("Iteration: " + algorithm.Iteration);

        var phrasee = new string(x.Genes);
        Console.WriteLine(phrasee);

        if (phrasee == phrase)
        {
          algorithm.IsRunning = false;
        }
      });

      var cancellationTokenSource = new CancellationTokenSource();
      var token = cancellationTokenSource.Token;

      algorithm.Run(token);
    }

    #region GetGene

    public char GetGene(string key)
    {
      int num = algorithm.GetRandomNext(0, chars.Length - 1);

      return chars[num];
    }

    #endregion

    #region GetScore

    public float GetScore(char[] genes)
    {
      var vsd = new string(genes);

      return vsd.Similarity(phrase, ignorCase: false);

    } 

    #endregion
  }
}