namespace xjtf.d;

public static class CommandArgsType
{
    public static CheckResult Checks(Command cmdType, CommandArgs? cmdArgs = null)
    {
        return Checks(cmdType, cmdArgs?._arguments ?? new());
    }

    private static CheckResult Checks(Command cmdType, List<object?> cmdArgs)
    {
        return cmdType switch
        {
            Command.Nop                  => CheckNopArgs(cmdArgs),
            Command.GetServices          => CheckGetServicesArgs(cmdArgs),
            Command.GetService           => CheckGetServiceArgs(cmdArgs),
            Command.StartService         => CheckStartServiceArgs(cmdArgs),
            Command.StopService          => CheckStopServiceArgs(cmdArgs),
            Command.RestartService       => CheckRestartServiceArgs(cmdArgs),
            Command.ToggleServiceEnabled => CheckToggleServiceEnabledArgs(cmdArgs),
            Command.InstallService       => CheckInstallServiceArgs(cmdArgs),
            Command.UninstallService     => CheckUninstallServiceArgs(cmdArgs),
            
            _ => CheckResult.Fail("Command not supported")
        };
    }

    private static CheckResult CheckNopArgs(List<object?> cmdArgs)
    {
        return cmdArgs.Count == 0
            ? CheckResult.Success
            : CheckResult.Fail("Nop command expects no arguments, but some were provided", cmdArgs);
    }

    private static CheckResult CheckGetServicesArgs(List<object?> cmdArgs)
    {
        return cmdArgs.Count == 0
            ? CheckResult.Success
            : CheckResult.Fail("Get services command expects no arguments, but some were provided", cmdArgs);
    }

    private static CheckResult CheckGetServiceArgs(List<object?> cmdArgs)
    {
        return cmdArgs.Count >= 1
            && CheckString(cmdArgs[0])
            ? CheckResult.Success
            : CheckResult.Fail("Name of the service must be provided");

        // TODO: check optional args
    }

    private static CheckResult CheckStartServiceArgs(List<object?> cmdArgs)
    {
        return cmdArgs.Count >= 1
            && CheckString(cmdArgs[0])
            ? CheckResult.Success
            : CheckResult.Fail("Name of the service must be provided");

        // TODO: check optional args
    }

    private static CheckResult CheckStopServiceArgs(List<object?> cmdArgs)
    {
        return cmdArgs.Count == 1
            && CheckString(cmdArgs[0])
            ? CheckResult.Success
            : CheckResult.Fail("Name of the service must be provided");
    }

    private static CheckResult CheckRestartServiceArgs(List<object?> cmdArgs)
    {
        return cmdArgs.Count >= 1
            && CheckString(cmdArgs[0])
            ? CheckResult.Success
            : CheckResult.Fail("Name of the service must be provided");

        // TODO: check optional args
    }

    private static CheckResult CheckToggleServiceEnabledArgs(List<object?> cmdArgs)
    {
        return cmdArgs.Count == 1
            && CheckBoolean(cmdArgs[0])
            ? CheckResult.Success
            : CheckResult.Fail("Toggle value must be provided");
    }

    private static CheckResult CheckInstallServiceArgs(List<object?> cmdArgs)
    {
        return cmdArgs.Count >= 2
            && CheckString(cmdArgs[0])
            && CheckString(cmdArgs[1])
            ? CheckResult.Success
            : CheckResult.Fail("Executable name and service install name must be provided");

        // TODO: check optional args
    }

    private static CheckResult CheckUninstallServiceArgs(List<object?> cmdArgs)
    {
        throw new NotImplementedException();
    }

    private static bool CheckString(object? argument)
    {
        return argument is string str_argument && str_argument != "";
    }
    
    private static bool CheckBoolean(object? argument)
    {
        return argument is bool bool_argument;
    }
}

public sealed class CheckResult
{
    public bool Failed => !_st;

    private readonly bool _st;
    public readonly string? Error;
    public readonly object? ErrorObject;

    private CheckResult(bool success, string? error, object? errorObject)
    {
        _st = success; Error = error; ErrorObject = errorObject;
    }

    internal static CheckResult Success => new(true, null, null);
    internal static CheckResult Fail(string error, object? obj = null) => new(false, error, obj);

    public static implicit operator bool(CheckResult checkResult) => checkResult._st;
}