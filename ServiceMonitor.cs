namespace xjtf.d;

public class ServiceMonitor : BackgroundService, IDisposable
{
    private static GreenThread? _thread;
    private static DaemonDbContext? _dbContext;
    private static ILogger<ServiceMonitor>? _logger;
    private static readonly List<ServiceController> _services = new();
    private static readonly GreenThreadScheduler _scheduler = GreenThreadScheduler.Default;

    public ServiceMonitor(Init _, DaemonDbContext dbContext, ILogger<ServiceMonitor> logger)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public static IEnumerable<dynamic> GetServiceStatuses()
    {
        var dbEntries = _dbContext!.ServiceEntries.ToArray();
        try
        {
            Monitor.Enter(_services);

            for (int i = 0; i < _services.Count; i++)
            {
                var service = _services[i];
                if (service != null) {
                    var serviceName = service.ServiceName;
                    var serviceStatus = service.Status.ToString();
                    var dbEntry = dbEntries.FirstOrDefault(e => e.ServiceName == serviceName);
                    var managed = dbEntry != null && QueryServiceManaged(dbEntry.ServiceEntryId);
                    var disabled = dbEntry != null && !managed;

                    yield return new
                    {
                        serviceName = serviceName,
                        displayName = dbEntry?.DisplayName ?? service.DisplayName,
                        description = dbEntry?.Description,
                        status = serviceStatus,
                        managed = managed,
                        disabled = disabled
                    };
                }
            }
        }
        finally
        {
            if (Monitor.IsEntered(_services))
                Monitor.Exit(_services);
        }
    }

    private static bool QueryServiceManaged(int serviceId)
    {
        var states =
            from service_state in _dbContext!.ServiceEntryStates
            orderby service_state.ChangedOnUtc descending
            where service_state.ServiceEntry.ServiceEntryId == serviceId
            select service_state.DaemonManaged;

        return states.FirstOrDefault();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(async () =>
        {
            _thread = new(new GreenThreadStart(Work));

            _scheduler.Schedule(_thread);
            var scheduler = (IThreadScheduler<GreenThread>)_scheduler;
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

            while (!cts.IsCancellationRequested)
            {
                await scheduler.RunOnceAsync();
                await Task.Delay(1_000); // ms
            }
        },
        CancellationToken.None);
    }

    private static void Work()
    {
        try
        {
            Monitor.Enter(_services);
            _services.Clear();
            _services.AddRange(ServiceController.GetServices());
        }
        finally
        {
            if (Monitor.IsEntered(_services))
                Monitor.Exit(_services);
        }

        // reschedule self
        _thread = _thread!.Reschedule(_scheduler);
    }

    new public void Dispose()
    {
        _thread?.Dispose();
        base.Dispose();
    }
}