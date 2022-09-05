namespace xjtf.d;

public sealed class CommandRunnerRestAdapter : ICommandResultTransformer<JsonResult>
{
    private readonly CommandRunner _commandRunner;

    public CommandRunnerRestAdapter(CommandRunner commandRunner) => _commandRunner = commandRunner;

    public async Task<object> RunAsync(string base64String, ICommandResultTransformer? resultTransformer = null)
    {
        var commandObject = CommandPayload.Unwrap(base64String);
        if (commandObject != null)
            return await RunAsync(CommandObject: commandObject, resultTransformer);
        else
            throw new ArgumentException("Command could not be read");
    }

    public async Task<JsonResult> RunAsync((Command CommandType, CommandArgs CommandArguments)? CommandObject, ICommandResultTransformer? resultTransformer = null)
    {
        var finalTransformer = this as ICommandResultTransformer<JsonResult>;
        var intermediateTransformer = resultTransformer ?? ICommandResultTransformer.Default;
        var composedTransformer = ICommandResultTransformer.Composer(intermediateTransformer, finalTransformer);

        return (JsonResult)await _commandRunner.RunAsync(CommandObject, composedTransformer);
    }
    
    #region result transformer

    JsonResult ICommandResultTransformer<JsonResult>.RunTransform(object commandResult)
        =>
            new JsonResult(commandResult);

    object ICommandResultTransformer.RunTransform(object commandResult)
        =>
            ((ICommandResultTransformer<JsonResult>)this).RunTransform(commandResult);

    #endregion
}