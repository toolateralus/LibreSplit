using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using LibreSplit.IO.Config;
using LibreSplit.Timing;
using Newtonsoft.Json;

namespace LibreSplit.IO.Serialization;

public class Serializer {
  public RunData ReadRunData(string path, ConfigLoader configLoader) {
    configLoader.Set(ConfigKeys.LastLoadedSplits, path);
    path = Uri.UnescapeDataString(path);
    if (!File.Exists(path)) {
      configLoader.Set(ConfigKeys.LastLoadedSplits, "");
      throw new FileNotFoundException($"Couldn't find file {path}");
    }
    var text = File.ReadAllText(path);
    
    try  {
      return JsonConvert.DeserializeObject<RunData>(text) ?? throw new Exception("Failed to deserialize run data.");
    } catch (Exception e) {
      throw new JsonException($"Failed to deserialize run data from {path} : {e}");
    }
    
  }
  public bool Read<T>(string path, out T result) {
    result = default!;
    try {
      path = Uri.UnescapeDataString(path);
      if (!File.Exists(path)) {
        return false;
      }
      var text = File.ReadAllText(path);
      var temp = JsonConvert.DeserializeObject<T>(text, Settings[typeof(T)]);
      if (temp == null) {
        return false;
      }
      result = temp;
      return true;
    } catch (JsonSerializationException e) {
      Console.WriteLine($"An error has occured while deserializing {typeof(T)}. {e}");
      return false;
    }
  }
  public bool Write<T>(string path, T value) {
    ArgumentNullException.ThrowIfNull(value);
    try {
      path = Uri.UnescapeDataString(path);
      using var stream = new StreamWriter(path);
      stream.Write(JsonConvert.SerializeObject(value, Settings[typeof(T)]));
    } catch (JsonSerializationException e) {
      Console.WriteLine($"An error has occured while serializing {typeof(T)}. {e}");
      return true;
    }
    return false;
  }

  public Dictionary<Type, JsonSerializerSettings> Settings = new() {
    [typeof(List<LayoutItem>)] = new(){
      TypeNameHandling = TypeNameHandling.Auto,
      Formatting = Formatting.Indented,
    },
    [typeof(RunData)] = new() {
      Formatting = Formatting.Indented,
    },
  };
}