namespace xjtf.d;

[Authorize][EnableCors]
public class ServiceMonitorController : BaseController
{
    private readonly MonitorClutch _clutch;
    private readonly GetServiceWorker _getServiceWorker;
    private readonly GetServicesWorker _getServicesWorker;
    private readonly CommandRunnerRestAdapter _commandRunner;
    private readonly ServiceHostTransformer _hostTransformer;
    private readonly ServiceStatsTransformer _statsTransformer;
    private readonly ServiceTimeseriesAggregator _timeseriesAggregator;

    private ICommandResultTransformer Transformer
    {
        get
        {
            return ICommandResultTransformer.Composer(_statsTransformer, _hostTransformer);
        }
    }

    public ServiceMonitorController
    (
        MonitorClutch clutch,
        GetServiceWorker getServiceWorker,
        GetServicesWorker getServicesWorker,
        CommandRunnerRestAdapter commandRunner,
        ServiceHostTransformer hostTransformer,
        ServiceStatsTransformer statsTransformer,
        ServiceTimeseriesAggregator timeseriesAggregator
    )
    {
        _clutch = clutch;
        _commandRunner = commandRunner;
        _hostTransformer = hostTransformer;
        _getServiceWorker = getServiceWorker;
        _statsTransformer = statsTransformer;
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
        return await _commandRunner.RunAsync((Command.GetServices, new CommandArgs()), Transformer);
    }

    [Route("/Monitor/Service/{serviceName}")][HttpGet]
    public async Task<IActionResult> GetService(string serviceName)
    {
        return await _commandRunner.RunAsync((Command.GetService, new CommandArgs(serviceName)), Transformer);
    }

    [Route("/Monitor/Service/Stats/{serviceName}")][HttpGet]
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
