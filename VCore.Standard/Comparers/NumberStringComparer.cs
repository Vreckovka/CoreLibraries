﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VCore.Standard.Comparers
{
  public class NumberStringComparer : IComparer<string>
  {
    public int Compare(string x, string y)
    {
      var regex = new Regex(@"(\d+)");

      // run the regex on both strings
      var xRegexResult = regex.Match(x);
      var yRegexResult = regex.Match(y);

      // check if they are both numbers
      if (xRegexResult.Success && yRegexResult.Success)
      {
        return int.Parse(xRegexResult.Groups[1].Value).CompareTo(int.Parse(yRegexResult.Groups[1].Value));
      }

      // otherwise return as string comparison
      return x.CompareTo(y);
    }
  }
}