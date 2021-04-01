using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CSCore.CoreAudioAPI;
using CSCore.Win32;
using VCore;
using VCore.Standard;


namespace SoundManagement
{
  public class AudioDeviceManager : ViewModel, IMMNotificationClient
  {
    #region Fields

    string controllerExeName = "EndPointController.exe";
    string controllerExePath;
    private MMDeviceEnumerator mMDeviceEnumerator;

    #endregion

    private AudioDeviceManager()
    {
      controllerExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, controllerExeName);

      mMDeviceEnumerator = new MMDeviceEnumerator();
      mMDeviceEnumerator.RegisterEndpointNotificationCallback(this);

      RefreshAudioDevices();
    }

    #region Properties

    #region Instance

    private static AudioDeviceManager instance;

    public static AudioDeviceManager Instance
    {
      get
      {
        if (instance == null)
        {
          instance = new AudioDeviceManager();
        }

        return instance;
      }
    }

    #endregion

    #region SoundDevices

    private ObservableCollection<SoundDevice> soundDevices;

    public ObservableCollection<SoundDevice> SoundDevices
    {
      get { return soundDevices; }
      set
      {
        if (value != soundDevices)
        {
          soundDevices = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region SelectedSoundDevice

    private SoundDevice selectedSoundDevice;

    public SoundDevice SelectedSoundDevice
    {
      get { return selectedSoundDevice; }
      set
      {
        if (value != selectedSoundDevice)
        {
          selectedSoundDevice = value;

          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #endregion

    #region Commands

    #region OnSelectedDeviceChangedCommand

    private ActionCommand onSelectedDeviceChanged;

    public ICommand OnSelectedDeviceChangedCommand
    {
      get
      {
        return onSelectedDeviceChanged ??= new ActionCommand(OnSelectedDeviceChanged);
      }
    }

    private void OnSelectedDeviceChanged()
    {
      if (!fromEvent)
        SelectDevice(SelectedSoundDevice.Index);
    }

    #endregion

    #endregion

    #region Methods

    #region GetDevices

    private IEnumerable<SoundDevice> GetDevices()
    {
      var devices = new List<SoundDevice>();

      //if (System.IO.File.Exists(controllerExePath))
      //{
      //  var p = new Process
      //  {
      //    StartInfo =
      //    {
      //      UseShellExecute = false,
      //      RedirectStandardOutput = true,
      //      CreateNoWindow = true,
      //      FileName = controllerExePath,
      //      Arguments = "-f \"%d|%ws|%d|%d|%ws|%ws|%ws\""
      //    }
      //  };
      //  p.Start();
      //  p.WaitForExit();
      //  var stdout = p.StandardOutput.ReadToEnd().Trim();
      //}

      var ssd = mMDeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);

      for (int i = 0; i < ssd.Count; i++ )
      {
        var device = new SoundDevice(ssd[i])
        {
          Index = i + 1
        };

        devices.Add(device);
      }

      return devices;
    }

    #endregion

    #region SelectDevice

    private void SelectDevice(int id)
    {
      if (System.IO.File.Exists(controllerExePath))
      {
        var p = new Process
        {
          StartInfo =
          {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            FileName = controllerExePath,
            Arguments = id.ToString(CultureInfo.InvariantCulture)
          }
        };
        p.Start();
        p.WaitForExit();
      }
    }

    #endregion

    #region RefreshAudioDevices

    public void RefreshAudioDevices()
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        var devices = GetDevices().ToList();

        if (SoundDevices == null)
        {
          SoundDevices = new ObservableCollection<SoundDevice>(devices);
        }
        else
        {
          var notIns = devices.Where(p => SoundDevices.All(p2 => p2.ID != p.ID));

          foreach (var notIn in notIns)
          {
            SoundDevices.Add(notIn);
          }

          foreach (var device in devices)
          {
            var cachedDevice = SoundDevices.SingleOrDefault(x => device.ID == x.ID);
          }
        }

        var defaultEndPoint = mMDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render,Role.Multimedia);

        SelectedSoundDevice = SoundDevices.SingleOrDefault(x => x.ID == defaultEndPoint.DeviceID);
      });
    }

    #endregion

    #region OnDeviceStateChanged

    private bool fromEvent;
    public void OnDeviceStateChanged(string deviceId, DeviceState newState)
    {
      fromEvent = true;
      RefreshAudioDevices();

      if (newState == DeviceState.UnPlugged)
      {
        var removed = SoundDevices.Single(x => x.ID == deviceId);
        SoundDevices.Remove(removed);
      }

      fromEvent = false;
    }

    #endregion

    #region OnDeviceAdded

    public void OnDeviceAdded(string pwstrDeviceId)
    {
      fromEvent = true;
      RefreshAudioDevices();
      fromEvent = false;
    }

    #endregion

    #region OnDeviceRemoved

    public void OnDeviceRemoved(string deviceId)
    {
      fromEvent = true;
      RefreshAudioDevices();
      fromEvent = false;
    }

    #endregion

    #region OnDefaultDeviceChanged

    public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
    {
      fromEvent = true;
      RefreshAudioDevices();
      fromEvent = false;
    }

    #endregion

    #region OnPropertyValueChanged

    public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
    {
    }

    #endregion

    #endregion
  }
  }
