namespace xjtf.d;

#pragma warning disable CS8618
public class DaemonDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<ServiceControl> ServiceControls { get; set; }
    public DbSet<ServiceObservation> ServiceObservations { get; set; }

    public string ConnectionString { get; private init; }

    public DaemonDbContext(IHostEnvironment hostEnvironment)
    {
        var currentFolder = hostEnvironment.ContentRootPath;
        var databaseFile = Path.Join(currentFolder, "xjtf.d.db");
        ConnectionString = $"Data Source={databaseFile}";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(ConnectionString);
        base.OnConfiguring(options);
    }
}
public class ServiceObservation
{
    public int Id { get; set; }
    public DateTimeOffset Observed { get; set; }
    public string ServiceName { get; set; }
    public ServiceControl? ControlledInfo { get; set; }
    public string Status { get; set; }
}

public class ServiceControl
{
    public int Id { get; set; }
    public string ServiceName { get; set; }
    public bool StoreBased { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? Deleted { get; set; }
}
#pragma warning restore CS8618