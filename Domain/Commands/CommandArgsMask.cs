namespace xjtf.d;

public sealed class CommandArgsMask
{
    internal readonly List<(Type Type, bool Required)> _constraints = new();

    public CommandArgsMask(params (Type Type, bool Required)[] constraints) => _constraints.AddRange(constraints);

    public bool SatisfiedBy(List<object?> arguments)
    {
        if (_constraints.Count < arguments.Count)
            return false;

        for (int v = 0; v < arguments.Count; v++)
        {
            var argument = arguments[v];
            var constraint = _constraints[v];

            if (argument == null && constraint.Required)
                return false;

            if (!argument!.GetType().IsAssignableTo(constraint.Type))
                return false;
        }
        return true;
    }
}