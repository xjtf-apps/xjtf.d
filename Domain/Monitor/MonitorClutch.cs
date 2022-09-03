namespace xjtf.d;

public class MonitorClutch
{
    private MonitorClutchState _state;
    private readonly object _lock = new();
    
    public MonitorClutchState State
    {
        get
        {
            try
            {
                Monitor.Enter(_lock);
                return _state;
            }
            finally
            {
                if (Monitor.IsEntered(_lock))
                    Monitor.Exit(_lock);
            }
        }
        set
        {
            try
            {
                Monitor.Enter(_lock);
                _state = value;
            }
            finally
            {
                if (Monitor.IsEntered(_lock))
                    Monitor.Exit(_lock);
            }
        }
    }
    
    public MonitorClutch() : this(MonitorClutchState.Attached) { }
    public MonitorClutch(MonitorClutchState state) => _state = state;
}

public enum MonitorClutchState { Attached, Detached }
