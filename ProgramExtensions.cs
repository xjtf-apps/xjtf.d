namespace xjtf.d;

public static class ProgramExtensions
{
    /// <summary>
    /// Runs the app until it completes or a restart signal is raised. When the
    /// restart signal is raised the app starts again recursively.
    /// </summary>
    public static async Task RunRestartableAsync(this Func<WebApplication> appFactory, Restartable restartable)
    {
        var app = appFactory();
        var semaphore = restartable.Semaphore;
        var cancellation = new CancellationTokenSource();

        async Task RunUntilSignal() => await Task.Run(() => semaphore.Wait());
        async Task RunAppAndRestartOnSignal()
        {
            var sema_work = RunUntilSignal();
            var app_work = Task.Run(async () => await app.RunAsync(), cancellation.Token);
            await Task.WhenAny(new[] { app_work, sema_work });

            if (sema_work.IsCompleted)
            {
                semaphore.Reset();
                cancellation.Cancel();
                Program.ApplicationLifetime!.StopApplication();
                await appFactory.RunRestartableAsync(restartable);
            }
            else return;
        }
        await RunAppAndRestartOnSignal();
    }
}

/// <summary>
/// Provides a method to request an app restart.
/// </summary>
public sealed class Restartable : IDisposable
{
    public readonly ManualResetEventSlim Semaphore = new(initialState: false);
    public void RestartApp() => Semaphore.Set();

    public void Dispose()
    {
        ((IDisposable)Semaphore).Dispose();
    }
}
