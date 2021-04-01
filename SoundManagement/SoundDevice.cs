using System;
using CSCore.CoreAudioAPI;

namespace SoundManagement
{
  public class SoundDevice 
  {
    private readonly MMDevice mMDevice;

    public SoundDevice(MMDevice mMDevice)
    {
      this.mMDevice = mMDevice ?? throw new ArgumentNullException(nameof(mMDevice));
    }

    public int Index { get; set; }
    public DeviceState DeviceState => mMDevice.DeviceState;
    public string Description => mMDevice.FriendlyName;
    public string ID => mMDevice.DeviceID;
  }
}