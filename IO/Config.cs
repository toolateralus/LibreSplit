using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LibreSplit.IO;

public static class ConfigKeys {
  public const string LastLoadedSplits = "lastLoadedSplits";
  public const string LastLoadedLayout = "lastLoadedLayout";
  
}

public class ConfigLoader {
  private static readonly string configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/libresplit/libresplit.json";
  private JObject configTable = [];

  public void Save() {
    try {
      var json = JsonConvert.SerializeObject(configTable);
      File.WriteAllText(configPath, json);
    }
    catch (Exception e) {
      Logs.LogError($"Failed to save config : {e}");
    }
    Logs.LogInfo($"Saved config to {configPath}");
  }

  public void Set(string key, object value) {
    configTable[key] = JToken.FromObject(value);
    Save();
  }
  public void TryLoadSplits(out string? loadedFile) {
    loadedFile = null;
    if (TryGetValue(ConfigKeys.LastLoadedSplits, out var path)) {
      var pathString = path?.ToObject<string>() ?? throw new InvalidDataException("Couldn't read lastLoadedPath from config: it may have been altered externally.");
      if (pathString != "") {
        loadedFile = Path.GetFullPath(pathString);
      }
    }
  }
  public void TryLoadLayout(out string? loadedFile) {
    loadedFile = null;
    if (TryGetValue(ConfigKeys.LastLoadedLayout, out var path)) {
      var pathString = path?.ToObject<string>() ?? throw new InvalidDataException("Couldn't read lastLoadedPath from config: it may have been altered externally.");
      if (pathString != "") {
        loadedFile = Path.GetFullPath(pathString);
      }
    }
  }
  public JObject LoadOrCreate() {
    if (File.Exists(configPath)) {
      configTable = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(configPath)) ?? [];
      Logs.LogInfo($"Loaded config from {configPath}");
    }
    else {
      DirectoryInfo? parent = Directory.GetParent(configPath);
      if (parent != null)
        Directory.CreateDirectory(parent.FullName);

      File.Create(configPath).Close();
      File.WriteAllText(configPath, JsonConvert.SerializeObject(configTable, Formatting.Indented));
      Logs.LogInfo($"Created new config at {configPath}");
    }

    return configTable;
  }
  public bool TryGetValue(string key, out JToken? value) {
    value = null;
    return configTable?.TryGetValue(key, out value) ?? false;
  }
}
