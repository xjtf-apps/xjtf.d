namespace xjtf.d;

public class CommandRunner : ICommandResultTransformer
{
    public async Task<object> RunAsync(string base64String, ICommandResultTransformer? resultTransformer = null)
    {
        var commandObject = CommandPayload.Unwrap(base64String);
        if (commandObject != null)
            return await RunAsync(CommandObject: commandObject, resultTransformer);
        else
            throw new ArgumentException("Command could not be read");
    }

    public async Task<object> RunAsync((Command CommandType, CommandArgs CommandArguments)? CommandObject, ICommandResultTransformer? resultTransformer = null)
    {
        using var shell = PowerShell.Create();
        var cmd = CommandObject!.Value.CommandType;
        var args = CommandObject!.Value.CommandArguments;
        var transformer =
            (ICommandResultTransformer)this;
            
        if (resultTransformer != null)
            transformer = ICommandResultTransformer.Composer(transformer, resultTransformer);

        TypeCheckArguments(cmd, args);

        if (CommandObject.Value.TryGenerateScript(out string script))
        {
            shell.AddScript(script);
            var result = await shell.InvokeAsync();
            var expectedResult = CommandResults.ByType(cmd);

            switch (expectedResult)
            {
                case CommandExpectedResult.NoObject:
                    if (result.Count != 0) throw new CommandRunnerException();
                    else return transformer.RunTransform("Success");

                case CommandExpectedResult.ServiceController:
                    if (result.Count != 1) throw new CommandRunnerException(result[0].ToString());
                    else return transformer.RunTransform(result[0]);

                case CommandExpectedResult.ServiceControllerArray:
                    if (result.Count <= 1) throw new CommandRunnerException(result.Count == 1 ? result[0].ToString() : "");
                    else return transformer.RunTransform(new List<object>(result));

                default:
                    throw new NotSupportedException("Command result not supported");
            }
        }
        throw new CommandRunnerException("Couldn't generate command script");
    }

    private static void TypeCheckArguments(Command cmd, CommandArgs args)
    {
        var checkResult = CommandArgsType.Checks(cmd, args);
        if (checkResult.Failed)
            throw new CommandRunnerException(checkResult.Error!);
    }

    object ICommandResultTransformer.RunTransform(object commandResult)
    {
        var @this = ((ICommandResultTransformer)this);

        if (commandResult is string)
            return commandResult;

        if (commandResult is IEnumerable<object> emb_result)
            return emb_result.Select(item => @this.RunTransform(item)).ToArray();

        if (commandResult is PSObject pso_result)
        {
            var obj = new ExpandoObject();
            var dict = obj as IDictionary<string, object>;
            pso_result.Members
                .Where(m => CommandExpectedResult_.ServiceObjectMembers.Contains(m.Name))
                .Select(m => new { m.Name, m.Value })
                .Where(m => m.Value != null).ToList()
                .ForEach(m => dict.Add(m.Name, m.Value));

            return dict;
        }
        throw new InvalidOperationException();
    }
}

[System.Serializable]
public class CommandRunnerException : System.Exception
{
    public CommandRunnerException() { }
    public CommandRunnerException(string message) : base(message) { }
    public CommandRunnerException(string message, System.Exception inner) : base(message, inner) { }
    protected CommandRunnerException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}