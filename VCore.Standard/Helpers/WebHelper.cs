using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace VCore.Standard.Helpers
{
  public static class WebHelper
  {
    public static async Task<HttpWebResponse> GetResponseAsyncWithCancellationToken(this HttpWebRequest request, CancellationToken ct)
    {
      using (ct.Register(() =>
      {
        Debug.WriteLine("Get response cancelled");
        request.Abort();
      }, useSynchronizationContext: false))
      {
        var response = await request.GetResponseAsync();
        ct.ThrowIfCancellationRequested();
        return (HttpWebResponse)response;
      }
    }
  }
}