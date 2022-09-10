namespace xjtf.d;

public class CommandShell : IDisposable
{
    private readonly Command _cmd;
    private readonly PowerShell _shell;
    private bool ThrowOnNoData { get; set; } = false;
    private bool ThrowOnAnyError { get; set; } = false;
    private readonly CommandExpectedResult _expectedResult;

    internal CommandShell(Command cmd)
    {
        _cmd = cmd;
        _shell = PowerShell.Create();
        _expectedResult = CommandResults.ByType(cmd);
    }

    public void AddScript(string script)
    {
        _shell.AddScript(script);

        switch (_cmd)
        {
            case Command.InstallService:
                ThrowOnAnyError = true; break;

            case Command.GetService:
            case Command.GetServices:
                ThrowOnNoData = true; break;

            default: break;
        }
    }

    public async Task<PSDataCollection<PSObject>> InvokeAsync()
    {
        var results = await _shell.InvokeAsync();
        CheckShellErrors(_shell, results);
        return results;
    }

    private void CheckShellErrors(PowerShell shell, PSDataCollection<PSObject> results)
    {
        var invocationInfo = shell.InvocationStateInfo;
        var exception = invocationInfo.Reason;
        
        if (exception != null)
            throw new CommandRunnerException("PowerShell execution exception", exception);
        
        if (shell.HadErrors && ThrowOnAnyError)
            throw new CommandRunnerException(results[0].ToString());

        if (results.Count == 0 && ThrowOnNoData)
            throw new CommandRunnerException("No data received.");
    }

    public void Dispose()
    {
        _shell.Dispose();
    }
}
