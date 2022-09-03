namespace xjtf.d;

public sealed class CommandRunnerRestAdapter
{
    private readonly CommandRunnerFactory _runnerFactory;
    public CommandRunner BaseCommandRunner => _runnerFactory.GetNew();
    public CommandRunnerRestAdapter(CommandRunnerFactory runnerFactory) => _runnerFactory = runnerFactory;

    public async Task<JsonResult> RunAsync(Command cmd, CommandArgs args)
    {
        var commandTask = BaseCommandRunner.RunAsync((cmd, args));
        await commandTask.WaitAsync(CancellationToken.None);
        if (commandTask.IsFaulted)
            return new JsonResult(commandTask.Exception!.Message);

        return new JsonResult(MapResult(commandTask.Result));
    }

    #region json serialization
    private static JsonResult MapResult(object result)
    {
        if (result is string str_result)
            return new JsonResult(str_result);

        if (result is IEnumerable<object> results)
            return new JsonResult(results.Select(r => MapResult(r)).ToArray());
        
        if (result is PSObject ps_result)
        {
            var obj = new ExpandoObject();
            var obj_dict = (IDictionary<String,Object>)obj!;
            var members = ps_result.Members
                .Where(m => Members.Contains(m.Name))
                .Select(m => new { Name = m.Name, Value = m.Value })
                .Where(m => m.Value != null)
                .ToList();

            members.ForEach(m => obj_dict.Add(m.Name, m.Value));
            return new JsonResult(obj);
        }
        return new JsonResult(new {});
    }

    private static HashSet<string> Members = new()
    {
        "UserName",
        "Description",
        "DelayedAutoStart",
        "BinaryPathName",
        "StartupType",
        "ServiceName",
        "CanPauseAndContinue",
        "CanShutdown",
        "CanStop",
        "DisplayName",
        "StartType",
        "Status",
        "ServiceType",
    };
    #endregion
}