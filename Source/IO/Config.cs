using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LibreSplit.IO.Config;
public static class ConfigKeys {
  public const string LastLoadedSplits = "lastLoadedSplits";
}
public class ConfigLoader {
  private static readonly string configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/libresplit/libresplit.json";
  private JObject configTable = [];
  
  public void Save() {
    try {
      var json = JsonConvert.SerializeObject(configTable);
      File.WriteAllText(configPath, json);
    } catch (Exception e) {
      Console.WriteLine($"Failed to save config : {e}");
    }
  }
  
  public void Set(string key, object value) {
    configTable[key] = JToken.FromObject(value);
    Save();
  }
  public void TryLoadSplits(out string? loadedFile) {
    loadedFile = null;
    if (TryGetValue(ConfigKeys.LastLoadedSplits, out var path)) {
      var pathString = path?.ToObject<string>() ?? throw new InvalidDataException("Couldn't read lastLoadedPath from config: it may have been altered externally.");
      loadedFile = Path.GetFullPath(pathString);
    }
  }
  public JObject LoadOrCreate() {
    if (File.Exists(configPath)) {
      configTable = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(configPath)) ?? [];
    }
    else {
      DirectoryInfo? parent = Directory.GetParent(configPath);
      if (parent != null)
        Directory.CreateDirectory(parent.FullName);

      File.Create(configPath).Close();
      File.WriteAllText(configPath, JsonConvert.SerializeObject(configTable, Formatting.Indented));
    }
    
    return configTable;
  }
  public bool TryGetValue(string key, out JToken? value) {
    value = null;
    return configTable?.TryGetValue(key, out value) ?? false;
  }
}