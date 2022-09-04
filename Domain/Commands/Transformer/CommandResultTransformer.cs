namespace xjtf.d;

public interface ICommandResultTransformer
{
    object RunTransform(object commandResult);
    Task<object> RunTransformAsync(Task<object> commandResult);
    IEnumerable<object> RunTransform(IEnumerable<object> commandResults) => commandResults.Select(cmdR => RunTransform(cmdR));
    async Task<IEnumerable<object>> RunTransformAsync(Task<IEnumerable<object>> commandResults) => (await commandResults).Select(cmdR => RunTransform(cmdR));

    static readonly ICommandResultTransformer Default = new CommandResultTransformer();
    static readonly Func<ICommandResultTransformer,ICommandResultTransformer,ComposedResultTransformer> Composer = (first, second) => new ComposedResultTransformer(first, second);
}

public interface ICommandResultTransformer<TResult> : ICommandResultTransformer
{
    new TResult RunTransform(object commandResult);
    new Task<TResult> RunTransformAsync(Task<object> commandResult);
    new IEnumerable<TResult> RunTransform(IEnumerable<object> commandResults) => commandResults.Select(cmdR => RunTransform(cmdR));
    new async Task<IEnumerable<TResult>> RunTransformAsync(Task<IEnumerable<object>> commandResults) => (await commandResults).Select(cmdR => RunTransform(cmdR));
}

internal class CommandResultTransformer : ICommandResultTransformer<object>
{
    public object RunTransform(object commandResult) => commandResult;
    public async Task<object> RunTransformAsync(Task<object> commandResult) => await commandResult;
}