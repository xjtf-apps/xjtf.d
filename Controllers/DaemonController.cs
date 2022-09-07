namespace xjtf.d;

[Authorize][EnableCors]
public class DaemonController : BaseController
{
    private readonly Restartable _restartable;
    private readonly ILogger<DaemonController> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public DaemonController
    (
        Restartable restartable,
        ILogger<DaemonController> logger,
        IHostApplicationLifetime applicationLifetime
    )
    {
        _logger = logger;
        _restartable = restartable;
        _applicationLifetime = applicationLifetime;
    }

    [Route("/Daemon/Status")][HttpGet]
    public IActionResult GetStatus()
    {
        return Ok(CalculateStatus());
    }

    [Route("/Daemon/Reload")][HttpGet]
    public IActionResult ReloadApp(int? timeout = null)
    {
        _logger.LogInformation("Received reload request. Reloading...");

        if (timeout == null)
            _restartable.RestartApp();
        else
            Task.Delay((int)timeout * 1000).ContinueWith(_ => _restartable.RestartApp());

        return Ok(timeout);
    }

    [Route("/Daemon/Shutdown")][HttpGet]
    public IActionResult ShutdownApp(int? timeout = null)
    {
        _logger.LogInformation("Received shutdown request. Shuting down...");

        if (timeout == null)
            Task.Delay(50).ContinueWith(_ => _applicationLifetime.StopApplication());
        else
            Task.Delay((int)timeout * 1000).ContinueWith(_ => _applicationLifetime.StopApplication());

        return Ok(timeout);
    }

    private object CalculateStatus()
    {
        var now = DateTime.Now;
        var uptime = now - Process.GetCurrentProcess().StartTime;
        var version = Assembly.GetEntryAssembly()!.GetName().Version;

        return new {
            uptime = uptime.TotalSeconds,
            version = version
        };
    }
}