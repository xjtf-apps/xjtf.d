namespace xjtf.d;

public class DaemonDbContextFactory
{
    public readonly string ConnectionString;

    public DaemonDbContextFactory
    (
        IHostEnvironment hostEnvironment
    )
    {
        ConnectionString = DecideConnectionString(hostEnvironment);
    }

    public DaemonDbContext GetNew() => new DaemonDbContext(ConnectionString);

    public static string DecideConnectionString(IHostEnvironment hostEnvironment)
    {
        var currentFolder = hostEnvironment.ContentRootPath;
        var databaseFile = Path.Join(currentFolder, "xjtf.d.db");
        var connectionString = $"Data Source={databaseFile}";
        return connectionString;
    }
}