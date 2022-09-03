namespace xjtf.d;

public class MonitorWorker : BackgroundService, IDisposable
{
    private readonly MonitorResults _results;
    private readonly DaemonDbContext _dbContext;
    private readonly MonitorDataService _dataService;
    
    public MonitorWorker
    (
        Init _,
        MonitorResults results,
        DaemonDbContext dbContext,
        MonitorDataService dataService
    )
    {
        _results = results;
        _dbContext = dbContext;
        _dataService = dataService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var task = Task.Run(async () =>
        {
            while (true)
            {
                var stopwatch = new Stopwatch(); stopwatch.Start();
                var observation = await _dataService.ObserveServicesAsync();
                if (observation != null)
                {
                    await StoreObservation(observation, DateTimeOffset.Now);
                    SetObservationResults(observation);
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
