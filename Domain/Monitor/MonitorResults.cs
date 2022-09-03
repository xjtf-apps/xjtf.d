namespace xjtf.d;

public class MonitorResults
{
    public MonitorResults() { }
    private readonly ServicesObservation _lastServicesObservation = new();

    public ServicesObservation? LastObservation
    {
        get
        {
            try
            {
                Monitor.Enter(_lastServicesObservation);
                return new ServicesObservation(_lastServicesObservation.ToArray());
            }
            finally
            {
                if (Monitor.IsEntered(_lastServicesObservation))
                    Monitor.Exit(_lastServicesObservation);
            }
        }
        set
        {
            try
            {
                Monitor.Enter(_lastServicesObservation);
                _lastServicesObservation.Clear();
                _lastServicesObservation.AddRange(value!);
            }
            finally
            {
                if (Monitor.IsEntered(_lastServicesObservation))
                    Monitor.Exit(_lastServicesObservation);
            }
        }
    }
}