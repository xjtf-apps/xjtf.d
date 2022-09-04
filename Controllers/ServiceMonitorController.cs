namespace xjtf.d;

[Authorize][EnableCors]
public class ServiceMonitorController : ControllerBase
{
    private readonly MonitorClutch _clutch;
    private readonly DaemonDbContext _dbContext;
    private readonly GetServiceWorker _getServiceWorker;
    private readonly GetServicesWorker _getServicesWorker;
    private readonly CommandRunnerRestAdapter _commandRunner;
    private readonly ServiceTimeseriesAggregator _timeseriesAggregator;

    public ServiceMonitorController
    (
        MonitorClutch clutch,
        DaemonDbContext dbContext,
        GetServiceWorker getServiceWorker,
        GetServicesWorker getServicesWorker,
        CommandRunnerRestAdapter commandRunner,
        ServiceTimeseriesAggregator timeseriesAggregator
    )
    {
        _clutch = clutch;
        _dbContext = dbContext;
        _commandRunner = commandRunner;
        _getServiceWorker = getServiceWorker;
        _getServicesWorker = getServicesWorker;
        _timeseriesAggregator = timeseriesAggregator;
    }

    [Route("/Monitor/{toggle}")][HttpGet]
    public IActionResult ToggleMonitor(bool toggle)
    {
        var state = toggle
            ? MonitorClutchState.Attached
            : MonitorClutchState.Detached;
        
        _clutch.State = state;
        
        return new JsonResult(new { State = toggle });
    }

    [Route("/Monitor/Services")][HttpGet]
    public async Task<IActionResult> GetServices()
    {
        return await _commandRunner.RunAsync(Command.GetServices, new CommandArgs());
    }

    [Route("/Monitor/Service/{serviceName}")][HttpGet]
    public async Task<IActionResult> GetService(string serviceName)
    {
        return await _commandRunner.RunAsync(Command.GetService, new CommandArgs(serviceName));
    }

    [Route("/Monitor/Service/{serviceName}/Stats")][HttpGet]
    public async Task<IActionResult> GetServiceStats(string serviceName)
    {
        // TODO: cache service stats so that a
        //       digest can be retrieved when getting the/all service/s

        return new JsonResult(await _timeseriesAggregator.GenerateReportAsync(serviceName));
    }


    [Route("/Monitor/Services/Subscribe")][HttpGet]
    public async Task GetServicesSubscription()
    {
        await _getServicesWorker.ConfigureLongRunningTask(HttpContext);
    }


    [Route("/Monitor/Service/Subscribe/{serviceName}")][HttpGet]
    public async Task GetServiceSubscription(string serviceName)
    {
        await _getServiceWorker.ConfigureLongRunningTask(HttpContext, serviceName);
    }
}
