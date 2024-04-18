using System;
using System.Diagnostics;
using System.Reflection;
using Avalonia.Threading;
namespace LibreSplit.Timing;

public class Timer {
  private readonly Stopwatch stopwatch = new();
  public TimeSpan LastSplitTime { get; set; }
   = TimeSpan.Zero;
  private TimeSpan pauseTime = TimeSpan.Zero;

  public TimeSpan Elapsed {
    get { return stopwatch.Elapsed - pauseTime; }
  }
  public TimeSpan Delta {
    get { return Elapsed - LastSplitTime; }
  }

  public bool Running { get; internal set; }

  private System.Threading.Timer? dispatcherTimer;

  /// <summary>
  /// NOTE: This has a rather unconventional implementation.
  /// This starts a DispatcherTimer and sets up the Tick event
  /// so that your Func<TimeSpan> is invoked once every 16 milliseconds
  /// with the current elapsed time.
  /// 
  /// This should probably be replaced with a method.
  /// </summary>
  public void Updater(Action<TimeSpan> action) {
    if (dispatcherTimer != null) {
      throw new InvalidOperationException("Cannot subscribe several events to the Timing.Timer.Updater hook");
    }
    dispatcherTimer = new(delegate {
      if (Running) action(Elapsed);
    });
    dispatcherTimer.Change(0, 16);
  }


  public void Pause() {
    if (Running) {
      Running = false;
      pauseTime = Elapsed;
      stopwatch.Stop();
    }
    else {
      Running = true;
      stopwatch.Start();
    }
  }
  public void Start() {
    if (!Running) {
      Running = true;
      stopwatch.Start();
      LastSplitTime = Elapsed;
    }
  }

  public void Stop() {
    Running = false;
    stopwatch.Stop();
  }

  public void Reset() {
    Running = false;
    stopwatch.Reset();
    pauseTime = TimeSpan.Zero;
  }

}