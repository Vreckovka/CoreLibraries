using System;
using CSCore.CoreAudioAPI;

namespace SoundManagement
{
  public class SoundDevice 
  {

    public SoundDevice(MMDevice mMDevice)
    {
      MMDevice = mMDevice ?? throw new ArgumentNullException(nameof(mMDevice));
    }

    public MMDevice MMDevice { get; set; }
    public int Index { get; set; } = -1;
    public DeviceState DeviceState => MMDevice.DeviceState;
    public string Description => MMDevice.FriendlyName;
    public string ID => MMDevice.DeviceID;
  }
}