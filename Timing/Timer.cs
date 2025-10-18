using System;
using System.Diagnostics;
using LibreSplit.UI;
namespace LibreSplit.Timing;


/// <summary>
/// This is a class that encapsulates a speedrun timer used for RunData and fetching
/// relevant timing information.
/// </summary>
public class Timer : ViewModelBase {
  private readonly Stopwatch stopwatch = new();
  /// <summary>
  /// The last time a split was made, either at the start of the run
  /// or the end of the last segment
  /// </summary>
  public TimeSpan? LastSplitTime { get; set; } = null;
  private TimeSpan startTime = TimeSpan.Zero;

  public event Action<TimeSpan>? OnTick;


  /// <summary>
  /// The time since this run began.
  /// </summary>
  private TimeSpan elapsed = TimeSpan.Zero;
  public TimeSpan Elapsed {
    get => elapsed;
    private set {
      elapsed = value;
      OnPropertyChanged();
    }
  }
  /// <summary>
  /// The time since the last split time.
  /// </summary>
  public TimeSpan? Delta {
    get { return Elapsed - LastSplitTime; }
  }

  /// <summary>
  /// Is the timer running?
  /// </summary>
  public bool Running {
    get => running; internal set {
      running = value;
      OnPropertyChanged();
    }
  }

  private System.Threading.Timer? updaterTimer;
  private bool running;


  /// <summary>
  /// This starts a System.Threading.Timer and sets up the Elapsed event
  /// so that your Action<TimeSpan> is invoked once every 16 milliseconds
  /// with the current elapsed time.
  /// </summary>
  public void Initialize() {
    if (updaterTimer != null) {
      throw new InvalidOperationException("Cannot initialize a timer that is already running.");
    }
    updaterTimer = new(delegate {
      Elapsed = stopwatch.Elapsed + startTime;
      OnTick?.Invoke(Elapsed);
    });
    updaterTimer.Change(0, 16);
  }

  /// <summary>
  /// Pause the current timer and store the total elapsed time up to this point.
  /// </summary>
  public void Pause() {
    if (Running) {
      Running = false;
      stopwatch.Stop();
    }
    else {
      Running = true;
      stopwatch.Start();
    }
  }
  /// <summary>
  /// Start the timer.
  /// </summary>
  public void Start(TimeSpan? startTime = null) {
    if (!Running) {
      Running = true;
      stopwatch.Start();
      LastSplitTime = Elapsed;
      this.startTime = startTime ?? TimeSpan.Zero;
    }
  }

  /// <summary>
  /// Stop the timer.
  /// </summary>
  public void Stop() {
    Running = false;
    stopwatch.Stop();
  }

  /// <summary>
  /// Reset the timer and clear the pauseTime and lastSplitTime.
  /// </summary>
  public void Reset() {
    Running = false;
    stopwatch.Reset();
    startTime = TimeSpan.Zero;
    LastSplitTime = TimeSpan.Zero;
  }
}
