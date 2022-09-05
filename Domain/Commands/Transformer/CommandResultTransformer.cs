namespace xjtf.d;

public interface ICommandResultTransformer
{
    object RunTransform(object commandResult);

    static readonly ICommandResultTransformer Default = new CommandResultTransformer();
    static readonly Func<ICommandResultTransformer,ICommandResultTransformer,ComposedResultTransformer> Composer = (first, second) => new ComposedResultTransformer(first, second);
}

public interface ICommandResultTransformer<TResult> : ICommandResultTransformer
{
    new TResult RunTransform(object commandResult);
}

internal class CommandResultTransformer : ICommandResultTransformer<object>
{
    public object RunTransform(object commandResult) => commandResult;
}