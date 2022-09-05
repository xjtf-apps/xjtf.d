namespace xjtf.d;

public class ComposedResultTransformer : ICommandResultTransformer
{
    public readonly ICommandResultTransformer FirstTransformer;
    public readonly ICommandResultTransformer SecondTransformer;

    internal ComposedResultTransformer
    (
        ICommandResultTransformer firstTransformer,
        ICommandResultTransformer secondTransformer
    )
    {
        FirstTransformer = firstTransformer;
        SecondTransformer = secondTransformer;
    }

    public object RunTransform(object commandResult)
    {
        return SecondTransformer.RunTransform(FirstTransformer.RunTransform(commandResult));
    }
}
