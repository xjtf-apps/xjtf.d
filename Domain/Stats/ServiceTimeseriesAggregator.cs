namespace xjtf.d;

public class ServiceTimeseriesAggregator
{
    private readonly DaemonDbContext _dbContext;

    public ServiceTimeseriesAggregator(DaemonDbContext dbContext) => _dbContext = dbContext;

    public async Task<ServiceTimeseries> GenerateReportAsync(string serviceName)
    {
        var newestControl =
            (await _dbContext.ServiceControls
            .Where(control => control.ServiceName == serviceName)
            .Where(control => control.Deleted == null)
            .ToListAsync())
            .OrderByDescending(control => control.Created)
            .FirstOrDefault();

        return await PipeSavedReportData(newestControl != null
            ? await GenerateControlledServiceReportAsync(newestControl)
            : await GenerateUncontrolledServiceReportAsync(serviceName));
    }

    private async Task<ServiceTimeseries> GenerateControlledServiceReportAsync(ServiceControl serviceControl)
    {
        var observations =
            await _dbContext.ServiceObservations
            .Where(o => o.ServiceName == serviceControl.ServiceName)
            .Where(o => o.ControlledInfo != null && o.ControlledInfo.Id == serviceControl.Id)
            .ToListAsync();
        
        return ServiceTimeseries.FromData(serviceControl.ServiceName,
            observations.Select(o => ServiceTimeseriesExtensions.FromData(o.Status, o.Observed)));
    }

    private async Task<ServiceTimeseries> GenerateUncontrolledServiceReportAsync(string serviceName)
    {
        var observations =
            await _dbContext.ServiceObservations
            .Where(o => o.ServiceName == serviceName)
            .ToListAsync();

        return ServiceTimeseries.FromData(serviceName,
            observations.Select(o => ServiceTimeseriesExtensions.FromData(o.Status, o.Observed)));
    }

    private async Task<ServiceTimeseries> PipeSavedReportData(ServiceTimeseries timeseries)
    {
        await _dbContext.ServiceStatistics.AddAsync(new ServiceStatistic()
        {
            ServiceName = timeseries.ServiceName,
            Measured = DateTimeOffset.Now,
            MeanUptime = timeseries.Mean
        });
        await _dbContext.SaveChangesAsync();
        return timeseries;
    }
}