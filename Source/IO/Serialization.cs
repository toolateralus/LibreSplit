using System;
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
      throw new FileNotFoundException($"Couldn't find file {path}");
    }
    var text = File.ReadAllText(path);
    
    try  {
      return JsonConvert.DeserializeObject<RunData>(text) ?? throw new Exception("Failed to deserialize run data.");
    } catch (Exception e) {
      throw new JsonException($"Failed to deserialize run data from {path} : {e}");
    }
    
  }
  public async Task WriteRunData(string path, RunData run, ConfigLoader configLoader) {
    configLoader.Set(ConfigKeys.LastLoadedSplits, path);
    path = Uri.UnescapeDataString(path);
    using var stream = new StreamWriter(path);
    try {
      await stream.WriteAsync(JsonConvert.SerializeObject(run, Formatting.Indented));
    }
    catch (JsonSerializationException e) {
      Console.WriteLine($"An error has occured while serializing segment data. {e}");
    }
    stream.Close();
  }
}