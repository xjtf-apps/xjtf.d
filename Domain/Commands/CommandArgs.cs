namespace xjtf.d;

public sealed class CommandArgs
{
    internal readonly List<object?> _arguments = new();

    public CommandArgs(params object?[] arguments) => _arguments.AddRange(arguments);
}
