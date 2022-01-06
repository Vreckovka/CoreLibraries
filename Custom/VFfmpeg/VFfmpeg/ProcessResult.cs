using System.Collections.Generic;

namespace StreamBroker
{
  public class ProcessResult
  {
    public IEnumerable<string> Output { get; set; }
    public int ResultCode { get; set; }
  }
}