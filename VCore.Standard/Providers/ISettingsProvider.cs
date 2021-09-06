using System.Collections.Generic;

namespace VCore.Standard.Providers
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