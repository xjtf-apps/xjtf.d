namespace xjtf.d;

public sealed class CommandRunner
{
    public async Task<object> RunAsync(string base64String)
    {
        var commandObject = CommandPayload.Unwrap(base64String);
        if (commandObject != null)
            return await RunAsync(CommandObject: commandObject);
        else
            throw new ArgumentException("Command could not be read");
    }

    public async Task<object> RunAsync((Command CommandType, CommandArgs CommandArguments)? CommandObject)
    {
        using var shell = PowerShell.Create();
        var cmd = CommandObject!.Value.CommandType;
        var args = CommandObject!.Value.CommandArguments;

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
                    else return "Success";

                case CommandExpectedResult.ServiceController:
                    if (result.Count != 1) throw new CommandRunnerException(result[0].ToString());
                    else return result[0];

                case CommandExpectedResult.ServiceControllerArray:
                    if (result.Count <= 1) throw new CommandRunnerException(result.Count == 1 ? result[0].ToString() : "");
                    else return new List<object>(result);

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