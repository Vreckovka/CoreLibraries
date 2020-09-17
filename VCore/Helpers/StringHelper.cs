using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCore
{
  public static class StringHelper
  {
    public static void EnsureDirectoryExists(this string filePath)
    {
      FileInfo fi = new FileInfo(filePath);

      if (!fi.Directory.Exists)
      {
        System.IO.Directory.CreateDirectory(fi.DirectoryName);
      }
    }
  }
}
