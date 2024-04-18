using System;
using System.Diagnostics;
namespace LibreSplit.Timing;

public class Timer {
  private readonly Stopwatch stopwatch = new();
  private TimeSpan lastSplitTime;
  public TimeSpan Elapsed {
    get { return stopwatch.Elapsed; }
  }
  public TimeSpan Delta {
    get {return Elapsed - lastSplitTime; }
  }

  
  public void Start() {
    stopwatch.Start();
    lastSplitTime = Elapsed;
  }
  
  public void Stop() {
    stopwatch.Stop();
  }
  
  public void Reset() {
    stopwatch.Reset();
  }

}