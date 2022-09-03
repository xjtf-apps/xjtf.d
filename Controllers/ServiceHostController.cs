namespace xjtf.d;

[Authorize]
public class ServiceHostController : ControllerBase
{
    private readonly ServiceStore _serviceStore;
    private readonly CommandRunnerRestAdapter _commandRunner;

    public ServiceHostController
    (
        ServiceStore serviceStore,
        CommandRunnerRestAdapter commandRunner
    )
    {
        _serviceStore = serviceStore;
        _commandRunner = commandRunner;
    }

    [Route("/Host/Service/Install")][HttpPost]
    public async Task<IActionResult> InstallServiceAsync([FromBody]InstallServiceRequest installRequest)
    {
        var store = _serviceStore.AccessStore(installRequest.StoreName);
        var installData = installRequest.GetInstallData();
        if (installData != null && !store.Empty)
        {
            var installFolder = store.InstallFolder;
            var commandArgs = installData.Value.CommandArgsBuilder(installFolder);
            return await _commandRunner.RunAsync(Command.InstallService, commandArgs);
        }
        else return BadRequest();
    }
}

#pragma warning disable CS8618
public class InstallServiceRequest
{
    [JsonPropertyName("storename")] public string StoreName { get; set; }

    [JsonPropertyName("serviceName")] public string? ServiceName { get; set; }
    [JsonPropertyName("displayName")] public string? DisplayName { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }

    [JsonPropertyName("executable")] public string Executable { get; set; }
    [JsonPropertyName("arguments")] public string? Arguments { get; set; }

    [JsonPropertyName("startupType")] public StartupType StartupType { get; set; }
}

public enum StartupType
{
    Automatic,
    AutomaticDelayedStart,
    Disabled,
    InvalidValue,
    Manual
}

public static class InstallServiceExtension
{
    public static (string StoreName, Func<string,CommandArgs> CommandArgsBuilder)? GetInstallData(this InstallServiceRequest installRequest)
    {
        if (installRequest.StoreName == null)
            return null;

        if (installRequest.Executable == null)
            return null;

        if (installRequest.ServiceName == null)
            installRequest.ServiceName = installRequest.StoreName;

        var binaryPathName = installRequest.Arguments != null
            ? $"{installRequest.Executable} {installRequest.Arguments}"
            : $"{installRequest.Executable}";


        CommandArgs builder(string installFolder)
        {
            return new CommandArgs
            (
                installRequest.ServiceName,
                $"'{installFolder}\\{binaryPathName}'",
                installRequest.DisplayName,
                installRequest.Description,
                installRequest.StartupType
            );
        }

        return (installRequest.StoreName, builder);
    }
}
#pragma warning restore CS8618