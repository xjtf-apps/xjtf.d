namespace xjtf.d;

public class DaemonDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<ServiceEntry> ServiceEntries { get; set; }
    public DbSet<ServiceEntryState> ServiceEntryStates { get; set; }
    public DbSet<ServiceEntryActual> ServiceEntryActuals { get; set; }

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
    }
}