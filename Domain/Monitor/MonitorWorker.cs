namespace xjtf.d;

public class MonitorWorker : BackgroundService, IDisposable
{
    private readonly MonitorClutch _clutch;
    private readonly MonitorResults _results;
    private readonly DaemonDbContext _dbContext;
    private readonly MonitorService _service;
    
    public MonitorWorker
    (
        Init _,
        MonitorClutch clutch,
        MonitorResults results,
        DaemonDbContext dbContext,
        MonitorService service,
    )
    {
        _clutch = clutch;
        _results = results;
        _dbContext = dbContext;
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var task = Task.Run(async () =>
        {
            while (true)
            {
                if (_clutch.State == MonitorClutchState.Attached)
                {
                    var observation = await _service.ObserveServicesAsync();
                    if (observation != null)
                    {
                        await StoreObservation(observation, DateTimeOffset.Now);
                        SetObservationResults(observation);
                    }
                }
                await Task.Delay(60_000);
            }
        });
        await task.WaitAsync(stoppingToken);
    }

    private async Task StoreObservation(ServicesObservation observation, DateTimeOffset time)
    {
        var tasks = observation.Select(async (svc_observation) => new ServiceObservation()
        {
            Observed = time,
            ServiceName = svc_observation.ServiceName,
            Status = svc_observation.ServiceStatusObserved,
            ControlledInfo = await _dbContext.ServiceControls.FirstOrDefaultAsync(svc => svc.ServiceName == svc_observation.ServiceName)
        });
        var observations = await Task.WhenAll(tasks);

        await _dbContext.ServiceObservations.AddRangeAsync(observations);
        await _dbContext.SaveChangesAsync();
    }

    private void SetObservationResults(ServicesObservation observation)
    {
        _results.LastObservation = observation;
    }
}
