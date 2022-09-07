namespace xjtf.d;

public class AuditWorker : BackgroundService, IDisposable
{
    private readonly DaemonDbContext _dbContext;
    
    public AuditWorker
    (
        Init _,
        DaemonDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var task = Task.Run(async () =>
        {
            while (true)
            {
                AuditLogWriter.Instance.Persist(_dbContext);
                await Task.Delay(1_000);
            }
        });
        await task.WaitAsync(stoppingToken);
    }
}
