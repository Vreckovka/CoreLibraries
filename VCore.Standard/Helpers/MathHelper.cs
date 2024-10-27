using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VCore.Standard.Helpers
{
  public static class MathHelper
  {

    public static float HarmonicMean(List<float> values)
    {
      float sumOfReciprocals = values.Sum(x => 1.0f / x);
      return values.Count / sumOfReciprocals;
    }

    public static float Mean(List<float> values)
    {
      return values.Average();
    }

    public static float GeometricMean(List<float> values)
    {

      double logSum = 0.0;
      int count = values.Count;

      foreach (float number in values)
      {
        logSum += Math.Log(Math.Max(number, 0.0000001)); // Sum of logarithms
      }

      // Exponentiate the average of the log values to get the geometric mean
      float geometricMean = (float)Math.Exp(logSum / count);

      return geometricMean;
    }


    #region GetMedian

    public static float GetMedian(float[] sourceNumbers)
    {
      //Framework 2.0 version of this method. there is an easier way in F4        
      if (sourceNumbers == null || sourceNumbers.Length == 0)
        throw new System.Exception("Median of empty array not defined.");

      //make sure the list is sorted, but use a new array
      float[] sortedPNumbers = (float[])sourceNumbers.Clone();
      Array.Sort(sortedPNumbers);

      //get the median
      int size = sortedPNumbers.Length;
      int mid = size / 2;
      float median = (size % 2 != 0) ? sortedPNumbers[mid] : (sortedPNumbers[mid] + sortedPNumbers[mid - 1]) / 2;
      return median;
    }

    #endregion

    public static decimal NormalizedValue(decimal value, decimal min, decimal max)
    {
      var result = (value - min) / (max - min);

      return ClampValue(result, min, max);
    }

    public static decimal ClampValue(decimal value, decimal min, decimal max)
    {
      return (decimal)Math.Max(min, Math.Min(max, value));
    }

    public static float ClampValue(float value, float min, float max)
    {
      return (float)Math.Max(min, Math.Min(max, value));
    }

    public static decimal CalculateMean(IList<decimal> values)
    {
      return values.Sum() / values.Count;
    }

    public static decimal CalculateStdDev(IList<decimal> values, decimal mean)
    {
      decimal sumSquaredDiffs = values.Sum(value => (value - mean) * (value - mean));
      return (decimal)Math.Sqrt((double)(sumSquaredDiffs / values.Count));
    }

    public static float NormalizeAndClampZScore(decimal value, decimal mean, decimal stdDev)
    {
      if (stdDev == 0) return 0;

      float zScore = (float)((value - mean) / stdDev);

      // Optionally clamp the Z-score
      return ClampValue(zScore, -10f, 10f);
    }

    public static float CalculateEMA(IList<float> values)
    {
      if (values == null || values.Count == 0)
        throw new ArgumentException("The list of values cannot be null or empty.");

      int periods = values.Count;  // Use the entire list as the period
      float multiplier = 2.0f / (periods + 1);  // Smoothing factor (alpha)

      // Start by using the first value as the initial EMA
      float ema = values[0];

      // Calculate EMA for the rest of the values
      for (int i = 1; i < values.Count; i++)
      {
        float currentValue = values[i];
        ema = (currentValue - ema) * multiplier + ema;
      }

      return ema;  // Return the most recent EMA value
    }

  }
}
