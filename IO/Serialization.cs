using System;
using System.Collections.Generic;
using System.IO;
using LibreSplit.Layouts;
using LibreSplit.Timing;
using Newtonsoft.Json;

namespace LibreSplit.IO;

public class Serializer {
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
    }
    catch (JsonSerializationException e) {
      Logs.LogError($"An error has occured while deserializing {typeof(T)}. {e}");
      return false;
    }
    catch (JsonReaderException e) {
      Logs.LogError($"An error has occured while deserializing {typeof(T)}. {e}");
      return false;
    }
  }
  public bool Write<T>(string path, T value) {
    ArgumentNullException.ThrowIfNull(value);
    try {
      path = Uri.UnescapeDataString(path);
      using var stream = new StreamWriter(path);
      stream.Write(JsonConvert.SerializeObject(value, Settings[typeof(T)]));
    }
    catch (JsonSerializationException e) {
      Logs.LogError($"An error has occured while serializing {typeof(T)}. {e}");
      return false;
    }
    return true;
  }

  public bool HasUnsavedChanges<T>(string oldPath, T? current) {
    if (!File.Exists(oldPath)) {
      return true;
    }
    var oldJson = File.ReadAllText(oldPath);
    var currentJson = JsonConvert.SerializeObject(current, Settings[typeof(T)]);
    return !string.Equals(oldJson, currentJson, StringComparison.Ordinal);
  }

  public Dictionary<Type, JsonSerializerSettings> Settings = new() {
    [typeof(LayoutData)] = new() {
      TypeNameHandling = TypeNameHandling.Auto,
      Formatting = Formatting.Indented,
    },
    [typeof(RunData)] = new() {
      Formatting = Formatting.Indented,
    },
  };
}
