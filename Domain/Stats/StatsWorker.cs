namespace xjtf.d;

public class StatsWorker : BackgroundService, IDisposable
{
    private readonly DaemonDbContext _dbContext;
    private readonly ServiceTimeseriesAggregator _timeseriesAggregator;

    public StatsWorker
    (
        Init _,
        DaemonDbContext dbContext,
        ServiceTimeseriesAggregator timeseriesAggregator
    )
    {
        _dbContext = dbContext;
        _timeseriesAggregator = timeseriesAggregator;
    }

    private GreenThreadStart GetThreadStart(string serviceName)
    {
        var threadWork = _timeseriesAggregator.GenerateReportAsync(serviceName);
        var threadStart = new GreenThreadStart(() => threadWork.RunSynchronously());
        return threadStart;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var task = Task.Run(async () =>
        {
            while (true)
            {
                {
                    // get distinct services from observations
                    // generate report for each one
                    var workScheduler = new GreenThreadScheduler() { StopWhenEmpty = true };
                    var workSchedulerCancellation = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

                    _dbContext
                        .ServiceObservations.Take(1_000).ToList()
                        .DistinctBy(observation => observation.ServiceName)
                        .Select(observation => observation.ServiceName)
                        .Select(serviceName => GetThreadStart(serviceName))
                        .Select(threadStart => new GreenThread(threadStart))
                        .ToList()
                        .ForEach(thread => workScheduler.Schedule(thread));

                    workScheduler.Run(workSchedulerCancellation);
                }
                
                await Task.Delay(60 * 60 * 60 * 1000);
            }
        });
        await task.WaitAsync(stoppingToken);
    }
}
