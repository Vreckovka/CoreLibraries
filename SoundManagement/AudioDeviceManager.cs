using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CSCore.CoreAudioAPI;
using CSCore.Win32;
using VCore;
using VCore.Standard;
using VCore.WPF;


namespace SoundManagement
{
  public class AudioDeviceManager : ViewModel, IMMNotificationClient
  {
    #region Fields

    string controllerExeName = "EndPointController.exe";
    string controllerExePath;
    private MMDeviceEnumerator mMDeviceEnumerator;
    private AudioEndpointVolumeCallback audioEndpointVolumeCallback;
    private AudioEndpointVolume audioEndpoint;

    #endregion

    #region Constructors

    private AudioDeviceManager()
    {
      controllerExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, controllerExeName);

      mMDeviceEnumerator = new MMDeviceEnumerator();
      mMDeviceEnumerator.RegisterEndpointNotificationCallback(this);

      Task.Run(RefreshAudioDevices);
    }

    #endregion

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

    private ObservableCollection<SoundDevice> soundDevices = new ObservableCollection<SoundDevice>();

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

    public bool WasLoaded { get; private set; } = false;

    #region SelectedSoundDevice

    private SoundDevice selectedSoundDevice;

    public SoundDevice SelectedSoundDevice
    {
      get { return selectedSoundDevice; }
      set => SetSelectedSoundDevice(value, false);
    }

    #endregion

    #region ActualVolume

    private double actualVolume;

    public double ActualVolume
    {
      get { return actualVolume; }
      set
      {
        if (value != actualVolume)
        {

          SetVolume(false, value);
        }
      }
    }

    #endregion

    #region IsActualMuted

    private bool isActualMuted;

    public bool IsActualMuted
    {
      get { return isActualMuted; }
      set
      {
        if (value != isActualMuted)
        {
          isActualMuted = value;

          SetIsMuted(false, value);
        }
      }
    }

    #endregion

    #endregion

    #region Methods

    #region GetDevices

    private IEnumerable<SoundDevice> GetDevices()
    {
      var devices = new List<SoundDevice>();

      var ssd = mMDeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);

      for (int i = 0; i < ssd.Count; i++)
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
      VSynchronizationContext.InvokeOnDispatcher(() =>
      {
        var devices = GetDevices().ToList();

        if (SoundDevices == null)
        {
          WasLoaded = false;

          SoundDevices = new ObservableCollection<SoundDevice>(devices);
        }
        else
        {
          var notIns = devices.Where(p => SoundDevices.All(p2 => p2.ID != p.ID));

          foreach (var notIn in notIns)
          {
            SoundDevices.Add(notIn);
          }
        }

        UpdateIndexes(devices);


        var defaultEndPoint = mMDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        SetSelectedSoundDevice(SoundDevices.SingleOrDefault(x => x.ID == defaultEndPoint.DeviceID), true);

        WasLoaded = true;
      });
    }

    #endregion

    public void UpdateIndexes(List<SoundDevice> devices = null)
    {
      if (devices == null)
      {
        devices = GetDevices().ToList();
      }

      foreach (var device in SoundDevices)
      {
        var deviceDevice = devices.SingleOrDefault(x => x.ID == device.ID);

        if (deviceDevice != null)
          device.Index = deviceDevice.Index;
      }
    }

    #region SetSelectedSoundDevice

    public void SetSelectedSoundDevice(SoundDevice soundDevice, bool fromEvent)
    {
      if (soundDevice != selectedSoundDevice && soundDevice != null)
      {
        selectedSoundDevice = soundDevice;

        if (!fromEvent)
        {
          SelectDevice(soundDevice.Index);
        }

        RegisterVolume(selectedSoundDevice.MMDevice);
        RaisePropertyChanged(nameof(SelectedSoundDevice));
      }
    }

    #endregion

    #region RegisterVolume

    private void RegisterVolume(MMDevice mMDevice)
    {
      if (audioEndpointVolumeCallback != null && audioEndpoint != null)
      {
        audioEndpointVolumeCallback.NotifyRecived -= VolumeCallBack_NotifyRecived;

        audioEndpoint.UnregisterControlChangeNotify(audioEndpointVolumeCallback);

        //audioEndpoint.Dispose();
      }

      audioEndpoint = AudioEndpointVolume.FromDevice(mMDevice);

      audioEndpointVolumeCallback = new AudioEndpointVolumeCallback();

      audioEndpoint.RegisterControlChangeNotify(audioEndpointVolumeCallback);

      audioEndpointVolumeCallback.NotifyRecived += VolumeCallBack_NotifyRecived;

      SetVolume(true, audioEndpoint.MasterVolumeLevelScalar * 100);
      SetIsMuted(true, audioEndpoint.IsMuted);
    }

    #endregion

    #region SetVolume

    private void SetVolume(bool fromEvent, double value)
    {
      if (!fromEvent)
      {
        var scalarVolume = (float)(value / 100.0);

        if (scalarVolume > 1)
        {
          scalarVolume = 1;
        }
        else if (scalarVolume < 0)
        {
          scalarVolume = 0;
        }

        audioEndpoint?.SetMasterVolumeLevelScalar(scalarVolume, Guid.NewGuid());
      }

      if (actualVolume != value)
      {
        actualVolume = value;
        RaisePropertyChanged(nameof(ActualVolume));
      }

    }

    #endregion

    #region SetIsMuted

    private void SetIsMuted(bool fromEvent, bool value)
    {
      if (!fromEvent)
      {
        audioEndpoint?.SetMute(value, Guid.NewGuid());
      }

      if (isActualMuted != value)
      {
        isActualMuted = value;
        RaisePropertyChanged(nameof(IsActualMuted));
      }
    }

    #endregion

    #region VolumeCallBack_NotifyRecived

    private void VolumeCallBack_NotifyRecived(object sender, AudioEndpointVolumeCallbackEventArgs e)
    {
      VSynchronizationContext.InvokeOnDispatcher(() =>
      {
        SetVolume(true, e.MasterVolume * 100);
        SetIsMuted(true, e.IsMuted);
      });
    }

    #endregion

    #region OnDeviceStateChanged

    public void OnDeviceStateChanged(string deviceId, DeviceState newState)
    {
      VSynchronizationContext.InvokeOnDispatcher(() =>
      {
        if (newState == DeviceState.UnPlugged)
        {
          var removed = SoundDevices.SingleOrDefault(x => x.ID == deviceId);

          if (removed != null)
            SoundDevices.Remove(removed);
        }

        RefreshAudioDevices();

      });
    }

    #endregion

    #region OnDeviceAdded

    public void OnDeviceAdded(string pwstrDeviceId)
    {
      RefreshAudioDevices();
    }

    #endregion

    #region OnDeviceRemoved

    public void OnDeviceRemoved(string deviceId)
    {
      RefreshAudioDevices();
    }

    #endregion

    #region OnDefaultDeviceChanged

    public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
    {
      RefreshAudioDevices();
    }

    #endregion

    #region OnPropertyValueChanged

    public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
    {

    }

    #endregion

    #region Dispose

    public override void Dispose()
    {
      audioEndpointVolumeCallback.NotifyRecived -= VolumeCallBack_NotifyRecived;
      audioEndpoint.UnregisterControlChangeNotify(audioEndpointVolumeCallback);

      audioEndpoint.Dispose();
    }

    #endregion

    #endregion
  }
}
