using System.Collections.Generic;

namespace VCore.Standard.NewFolder
{
  public interface ISettingsProvider
  {
    bool Load();
    void Save();
    void AddOrUpdateSetting(string key, SettingParameters value);
    SettingParameters GetSetting(string key);
    IReadOnlyDictionary<string,SettingParameters> Settings { get; }
  }
}