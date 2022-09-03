namespace xjtf.d;

public sealed class CommandRunnerRestAdapterFactory
{
    private readonly CommandRunnerFactory _runnerFactory;
    public CommandRunnerRestAdapter GetNew() => new(_runnerFactory);
    public CommandRunnerRestAdapterFactory(CommandRunnerFactory runnerFactory) => _runnerFactory = runnerFactory;
}