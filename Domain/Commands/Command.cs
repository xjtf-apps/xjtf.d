namespace xjtf.d;

public enum Command
{
    Nop,
    GetServices,
    GetService,
    StartService,
    StopService,
    RestartService,
    ToggleServiceEnabled,
    InstallService,
    UninstallService
}

public enum CommandExpectedResult
{
    NoObject,
    ServiceController,
    ServiceControllerArray
}

public static class CommandResults
{
    public static CommandExpectedResult ByType(Command cmdType) => cmdType switch
    {
        Command.Nop              => CommandExpectedResult.NoObject,
        Command.GetServices      => CommandExpectedResult.ServiceControllerArray,
        Command.GetService       => CommandExpectedResult.ServiceController,
        Command.StartService     => CommandExpectedResult.NoObject,
        Command.StopService      => CommandExpectedResult.NoObject,
        Command.RestartService   => CommandExpectedResult.NoObject,
        Command.InstallService   => CommandExpectedResult.ServiceController,
        Command.UninstallService => CommandExpectedResult.NoObject,

        _ => throw new NotSupportedException("Command not supported")
    };
}

public static class CommandMasks
{
    public static CommandArgsMask ByType(Command cmdType) => cmdType switch
    {
        Command.Nop                  => NopMask,
        Command.GetServices          => GetServicesMask,
        Command.GetService           => GetServiceMask,
        Command.StartService         => StartServiceMask,
        Command.StopService          => StopServiceMask,
        Command.RestartService       => RestartServiceMask,
        Command.ToggleServiceEnabled => ToggleServiceEnabledMask,
        Command.InstallService       => InstallServiceMask,
        Command.UninstallService     => UninstallServiceMask,

        _ => throw new NotSupportedException(nameof(cmdType))
    };
    
    private static readonly CommandArgsMask NopMask = new();
    private static readonly CommandArgsMask GetServicesMask = new();
    private static readonly CommandArgsMask GetServiceMask = new((typeof(string), true));
    private static readonly CommandArgsMask StartServiceMask = new((typeof(string), true));
    private static readonly CommandArgsMask StopServiceMask = new((typeof(string), true));
    private static readonly CommandArgsMask RestartServiceMask = new((typeof(string), true));
    private static readonly CommandArgsMask ToggleServiceEnabledMask = new((typeof(bool), true));
    private static readonly CommandArgsMask InstallServiceMask =
        new(
            (typeof(string)     , true),  // Name
            (typeof(string)     , true),  // BinaryPathName
            (typeof(string)     , false), // DisplayName
            (typeof(string)     , false), // Description
            (typeof(StartupType), false)  // StartupType
        );
    private static readonly CommandArgsMask UninstallServiceMask = new();
}