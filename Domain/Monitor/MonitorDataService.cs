using System.Collections;

namespace xjtf.d;

public class MonitorDataService
{
    private readonly CommandRunnerFactory _commandRunnerFactory;

    public MonitorDataService(CommandRunnerFactory commandRunnerFactory)
    {
        _commandRunnerFactory = commandRunnerFactory;
    }

    public async Task<ServicesObservation> ObserveServicesAsync()
    {
        var commandArgs = new CommandArgs();
        var results = new ServicesObservation();
        var commandRunner = _commandRunnerFactory.GetNew();
        var services = (List<object>)await commandRunner.RunAsync((Command.GetServices, commandArgs));

        foreach (var service in services)
        {
            dynamic ps_service = (dynamic)service;
            results.Add(new ServiceStatus() {
                ServiceName = ps_service.ServiceName,
                ServiceStatusObserved = ps_service.Status.ToString()
            });
        }
        return results;
    }

    // TODO: ensure services started
}

#pragma warning disable CS8618
public class ServiceStatus : ValueObject<ServiceStatus>
{
    public string ServiceName { get; set; }
    public string ServiceStatusObserved { get; set; }
}

public class ServicesObservation : ICollection<ServiceStatus>
{
    private readonly List<ServiceStatus> _observations;
    
    public ServicesObservation() => _observations = new();
    public ServicesObservation(IEnumerable<ServiceStatus> statuses) => _observations = statuses.ToList();

    public int Count => ((ICollection<ServiceStatus>)_observations).Count;
    public bool IsReadOnly => ((ICollection<ServiceStatus>)_observations).IsReadOnly;

    public void Add(ServiceStatus item)
    {
        ((ICollection<ServiceStatus>)_observations).Add(item);
    }

    public void AddRange(IEnumerable<ServiceStatus> items)
    {
        items.ToList().ForEach(item => Add(item));
    }

    public void Clear()
    {
        ((ICollection<ServiceStatus>)_observations).Clear();
    }

    public bool Contains(ServiceStatus item)
    {
        return ((ICollection<ServiceStatus>)_observations).Contains(item);
    }

    public void CopyTo(ServiceStatus[] array, int arrayIndex)
    {
        ((ICollection<ServiceStatus>)_observations).CopyTo(array, arrayIndex);
    }

    public IEnumerator<ServiceStatus> GetEnumerator()
    {
        return ((IEnumerable<ServiceStatus>)_observations).GetEnumerator();
    }

    public bool Remove(ServiceStatus item)
    {
        return ((ICollection<ServiceStatus>)_observations).Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_observations).GetEnumerator();
    }
}
#pragma warning restore CS8618