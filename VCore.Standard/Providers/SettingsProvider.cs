using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace VCore.Standard.Providers
{
  public class SettingParameters
  {
    public SettingParameters(string value, bool canPickPath = false)
    {
      Value = value;
      CanPickPath = canPickPath;
    }

    public string Value { get; set; }
    public bool CanPickPath { get; set; }

    public override bool Equals(Object obj)
    {
      return (obj is SettingParameters) 
             && ((SettingParameters)obj).Value == Value
             && ((SettingParameters)obj).CanPickPath == CanPickPath;
    }
  }

  public class SettingsProvider : ISettingsProvider
  {
    private readonly string settingsPath;
    private object fileLock = new object();
    private Dictionary<string, SettingParameters> settings = new Dictionary<string, SettingParameters>();

    public IReadOnlyDictionary<string, SettingParameters> Settings
    {
      get
      {
        return settings;
      }
    }

    public SettingsProvider(string settingsPath)
    {
      this.settingsPath = settingsPath ?? throw new ArgumentNullException(nameof(settingsPath));
    }

    #region Load

    public bool Load()
    {
      try
      {
        lock (fileLock)
        {
          if (File.Exists(settingsPath))
          {
            var readAllText = File.ReadAllText(settingsPath);

            settings = JsonSerializer.Deserialize<Dictionary<string, SettingParameters>>(readAllText);

            return true;
          }
        }

        return false;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    #endregion

    #region Save

    public void Save()
    {
      lock (fileLock)
      {
        var json = JsonSerializer.Serialize(settings);
        FileStream stream = null;

        if (!File.Exists(settingsPath))
        {
          StringHelper.EnsureDirectoryExists(settingsPath);

          stream = File.Create(settingsPath);
        }

        stream?.Flush();
        stream?.Close();

        File.WriteAllText(settingsPath, json);
      }
    }

    #endregion

    #region AddOrUpdateSetting

    public void AddOrUpdateSetting(string key, SettingParameters value)
    {
      if (settings.TryGetValue(key, out var oldValue))
      {
        if (!Equals(oldValue, value))
        {
          settings[key] = value;
        }
      }
      else
      {
        settings.Add(key, value);

      }

      Save();
    }

    #endregion

    #region GetSetting

    public SettingParameters GetSetting(string key)
    {
      if (settings.TryGetValue(key, out var value))
      {
        return value;
      }
      else
      {
        return default;
      }
    }

    #endregion
  }
}
