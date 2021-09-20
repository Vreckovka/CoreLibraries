using System;
using System.Text.Json.Serialization;
using CSCore.CoreAudioAPI;

namespace SoundManagement
{
  public class SoundDevice 
  {
    public SoundDevice()
    {
      
    }

    public SoundDevice(MMDevice mMDevice)
    {
      MMDevice = mMDevice ?? throw new ArgumentNullException(nameof(mMDevice));
    }

    [JsonIgnore]
    public MMDevice MMDevice { get; set; }
    public int Index { get; set; } = -1;
    public DeviceState DeviceState => MMDevice.DeviceState;
    public string Description => MMDevice.FriendlyName;
    public string ID => MMDevice.DeviceID;
  }

  public class BlankSoundDevice
  {
    public string Description { get; set; }
    public string Id { get; set; }
    public int Priority { get; set; }
    public bool DisableAutomaticConnect { get; set; }

  }
}