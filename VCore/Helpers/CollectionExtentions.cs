using System.Collections.Generic;
using System.Linq;

namespace VCore.Helpers
{
  public static class CollectionExtentions
  {
    public static int GetSequenceHashCode<T>(this IEnumerable<T> sequence)
    {
      const int seed = 487;
      const int modifier = 31;

      unchecked
      {
        return sequence.Aggregate(seed, (current, item) =>
          (current * modifier) + item.GetHashCode());
      }
    }
  }
}