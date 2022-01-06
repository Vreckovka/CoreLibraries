using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VFfmpeg
{
  public class DebugOutputHandler : ProcessOutputHandler
  {
    public DebugOutputHandler(Process process) : base(process)
    {
    }

    #region OnNewOutputMessage

    protected override void OnNewOutputMessage(object sendingProcess, DataReceivedEventArgs outLine)
    {
      base.OnNewOutputMessage(sendingProcess, outLine);

      Debug.WriteLine(outLine.Data);
    }

    #endregion
  }

  public class ProcessOutputHandler : IDisposable
  {
    private readonly Process process;
    private List<string> outputs = new List<string>();

    public ProcessOutputHandler(Process process)
    {
      this.process = process ?? throw new ArgumentNullException(nameof(process));
    }

    private TaskCompletionSource<IEnumerable<string>> outputTask = new TaskCompletionSource<IEnumerable<string>>();
    public int? ExitCode { get; set; }

    public Task<IEnumerable<string>> StartProcessAndWaitForExit()
    {
      try
      {
        process.OutputDataReceived += new DataReceivedEventHandler(OnNewOutputMessage);
        process.ErrorDataReceived += new DataReceivedEventHandler(OnNewOutputMessage);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.WaitForExit();

        ExitCode = process.ExitCode;

      }
      catch (Exception ex)
      {

      }
      finally 
      {
        outputTask.SetResult(outputs);
      }

      return outputTask.Task;
    }
   

    #region OnNewOutputMessage

    protected virtual void OnNewOutputMessage(object sendingProcess, DataReceivedEventArgs outLine)
    {
      Debug.WriteLine(outLine.Data);
      outputs.Add(outLine.Data);
    }

    #endregion

    public void Dispose()
    {
      try
      {
        process.Kill();
        process.OutputDataReceived -= new DataReceivedEventHandler(OnNewOutputMessage);
        process.ErrorDataReceived -= new DataReceivedEventHandler(OnNewOutputMessage);
        process.Dispose();
      }
      catch (Exception ex)
      {
      }
    }
  }

  public class FFmpegFrameOutputMessage
  {
    public int Frame { get; set; }
    public string Fps { get; set; }
    public string Q { get; set; }
    public string Size { get; set; }
    public TimeSpan Time { get; set; }
    public string Bitrate { get; set; }
    public string Speed { get; set; }
  }

  public class FramesProcessOutputHandler : ProcessOutputHandler
  {
    private Subject<FFmpegFrameOutputMessage> subject = new Subject<FFmpegFrameOutputMessage>();

    public IObservable<FFmpegFrameOutputMessage> Frames
    {
      get
      {
        return subject.AsObservable();
      }
    }

    public FramesProcessOutputHandler(Process process) : base(process)
    {
    }

    protected override void OnNewOutputMessage(object sendingProcess, DataReceivedEventArgs outLine)
    {
      var message = outLine.Data;

      base.OnNewOutputMessage(sendingProcess, outLine);

      if (!string.IsNullOrEmpty(message) && message.Contains("frame"))
      {
        var regex = new Regex(@"[a-zA-Z0-9_.:\/?]+");
        var regexResult = regex.Matches(message);

        var frame = GetPropertyValue(regexResult, "frame");
        var fps = GetPropertyValue(regexResult, "fps");
        var q = GetPropertyValue(regexResult, "q");
        var size = GetPropertyValue(regexResult, "size");
        var time = GetPropertyValue(regexResult, "time");
        var bitrate = GetPropertyValue(regexResult, "bitrate");
        var speed = GetPropertyValue(regexResult, "speed");

        if (int.TryParse(frame, out var framesInt))
        {
          subject.OnNext(new FFmpegFrameOutputMessage()
          {
            Frame = framesInt,
            Bitrate = bitrate,
            Fps = fps,
            Q = q,
            Size = size,
            Speed = speed,
            Time = TimeSpan.Parse(time)
          });
        }
      }
    }

    private string GetPropertyValue(MatchCollection matchCollection, string propertyName)
    {
      var list = matchCollection.ToList();
      var frameHeader = list.SingleOrDefault(x => x.Value == propertyName);

      if (frameHeader != null)
      {
        var index = list.IndexOf(frameHeader);

        if (index + 1 < list.Count)
        {
          var frameValue = list[index + 1];

          return frameValue.Value;
        }
      }

      return null;
    }
  }
}