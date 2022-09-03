namespace xjtf.d;

[Authorize]
public class ServiceController : ControllerBase
{
    private readonly DaemonDbContext _dbContext;
    private readonly CommandRunnerRestAdapter _commandRunner;

    public ServiceController
    (
        DaemonDbContext dbContext,
        CommandRunnerRestAdapter commandRunner
    )
    {
        _dbContext = dbContext;
        _commandRunner = commandRunner;
    }

    [Route("/Service/Start/{serviceName}")][HttpGet]
    public async Task<IActionResult> StartServiceAsync(string serviceName)
    {
        return await _commandRunner.RunAsync(Command.StartService, new CommandArgs(serviceName));
    }

    [Route("/Service/Stop/{serviceName}")][HttpGet]
    public async Task<IActionResult> StopServiceAsync(string serviceName)
    {
        return await _commandRunner.RunAsync(Command.StopService, new CommandArgs(serviceName));
    }

    [Route("/Service/Restart/{serviceName}")][HttpGet]
    public async Task<IActionResult> RestartServiceAsync(string serviceName)
    {
        return await _commandRunner.RunAsync(Command.RestartService, new CommandArgs(serviceName));
    }

    [Route("/Service/Enable/{serviceName}")][HttpGet]
    public async Task<IActionResult> EnableServiceAsync(string serviceName)
    {
        return await _commandRunner.RunAsync(Command.ToggleServiceEnabled, new CommandArgs(true));
    }

    [Route("/Service/Disable/{serviceName}")][HttpGet]
    public async Task<IActionResult> DisableServiceAsync(string serviceName)
    {
        return await _commandRunner.RunAsync(Command.ToggleServiceEnabled, new CommandArgs(false));
    }
}