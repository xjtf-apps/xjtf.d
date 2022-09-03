namespace xjtf.d;

[Authorize]
public class ServiceMonitorController : ControllerBase
{
    private readonly DaemonDbContext _dbContext;
    private readonly GetServiceWorker _getServiceWorker;
    private readonly GetServicesWorker _getServicesWorker;
    private readonly CommandRunnerRestAdapter _commandRunner;
    private readonly ServiceTimeseriesAggregator _timeseriesAggregator;

    public ServiceMonitorController
    (
        DaemonDbContext dbContext,
        GetServiceWorker getServiceWorker,
        GetServicesWorker getServicesWorker,
        CommandRunnerRestAdapter commandRunner,
        ServiceTimeseriesAggregator timeseriesAggregator
    )
    {
        _dbContext = dbContext;
        _commandRunner = commandRunner;
        _getServiceWorker = getServiceWorker;
        _getServicesWorker = getServicesWorker;
        _timeseriesAggregator = timeseriesAggregator;
    }

    [Route("/Monitor/{toggle}")][HttpGet]
    public IActionResult ToggleMonitor(bool toggle)
    {
        // TODO: toggle monitor attached/detached
        return Ok();
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