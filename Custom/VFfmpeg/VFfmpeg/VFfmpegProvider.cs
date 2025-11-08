using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StreamBroker;
using VCore.Standard.Factories.ViewModels;

namespace VFfmpeg
{
  public interface IVFfmpegProvider
  {
    TProcessOutputHandler CreateFfmpeg<TProcessOutputHandler>(string argumetns) where TProcessOutputHandler : ProcessOutputHandler;
    Task<ProcessResult> RunFfmpeg<TProcessOutputHandler>(string argumetns,bool forceClosePrevious = false) where TProcessOutputHandler : ProcessOutputHandler;
    Task<ProcessResult> RunFfprobe<TProcessOutputHandler>(string argumetns) where TProcessOutputHandler : ProcessOutputHandler;

    string FFMPEG_PATH
    {
      get;
    }
  }

  public class VFfmpegProvider : IVFfmpegProvider
  {
    private readonly IViewModelsFactory viewModelsFactory;
    private readonly string ffmpegFolderPath;

    private string ffmpegPath;
    private string ffprobePath;

    public VFfmpegProvider(IViewModelsFactory viewModelsFactory, string ffmpegFolderPath)
    {
      this.viewModelsFactory = viewModelsFactory ?? throw new ArgumentNullException(nameof(viewModelsFactory));
      this.ffmpegFolderPath = ffmpegFolderPath ?? throw new ArgumentNullException(nameof(ffmpegFolderPath));

      ffmpegPath = Path.Combine(ffmpegFolderPath, "ffmpeg.exe");
      ffprobePath = Path.Combine(ffmpegFolderPath, "ffprobe.exe");
    }

    public string FFMPEG_PATH => ffmpegPath;

    #region CreateFfmpeg

    public TProcessOutputHandler CreateFfmpeg<TProcessOutputHandler>(string arguments) where TProcessOutputHandler : ProcessOutputHandler
    {
      return CreateProcess<TProcessOutputHandler>(ffmpegPath, arguments);
    }

    #endregion

    #region RunFfmpeg

    SemaphoreSlim ffmpegSemaphore = new SemaphoreSlim(1, 1);
    private IDisposable lastFfmpegProcess;
    public async Task<ProcessResult> RunFfmpeg<TProcessOutputHandler>(string arguments, bool forceClosePrevious = false) where TProcessOutputHandler : ProcessOutputHandler
    {
      try
      {
        if(forceClosePrevious && lastFfmpegProcess != null)
        {
          lastFfmpegProcess.Dispose();
        }

        await ffmpegSemaphore.WaitAsync();

        var result = new ProcessResult();

        var processOutputHandler = CreateProcess<TProcessOutputHandler>(ffmpegPath, arguments);
        lastFfmpegProcess = processOutputHandler;

        var output = await processOutputHandler.StartProcessAndWaitForExit();

        if (processOutputHandler.ExitCode != null)
          result.ResultCode = processOutputHandler.ExitCode.Value;

        result.Output = output;
        processOutputHandler.Dispose();
        lastFfmpegProcess = null;

        return result;
      }
      finally
      {
        ffmpegSemaphore.Release();
      }
    }

    #endregion

    #region RunFfprobe

    public Task<ProcessResult> RunFfprobe<TProcessOutputHandler>(string arguments) where TProcessOutputHandler : ProcessOutputHandler
    {
      return RunProcess<TProcessOutputHandler>(ffprobePath, arguments);
    }

    #endregion

    #region CreateProcess

    public TProcessOutputHandler CreateProcess<TProcessOutputHandler>(string exePath, string arguments) where TProcessOutputHandler : ProcessOutputHandler
    {
      var process = new Process();

      process.StartInfo = new ProcessStartInfo()
      {
        FileName = exePath,
        Arguments = arguments,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        CreateNoWindow = true
      };

      process.StartInfo.UseShellExecute = false;
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.RedirectStandardError = true;

      var processOutputHandler = viewModelsFactory.Create<TProcessOutputHandler>(process);

      return processOutputHandler;
    }

    #endregion

    #region RunProcess

    private Task<ProcessResult> RunProcess<TProcessOutputHandler>(string exePath, string arguments) where TProcessOutputHandler : ProcessOutputHandler
    {
      return Task.Run(async () =>
      {
        var result = new ProcessResult();

        var processOutputHandler = CreateProcess<TProcessOutputHandler>(exePath, arguments);

        var output = await processOutputHandler.StartProcessAndWaitForExit();

        if (processOutputHandler.ExitCode != null)
          result.ResultCode = processOutputHandler.ExitCode.Value;

        result.Output = output;
        processOutputHandler.Dispose();

        return result;
      });
    }

    #endregion
  }
}
