using Microsoft.EntityFrameworkCore;

public class XjtfDbContext : DbContext
{
    public XjtfDbContext(DbContextOptions<XjtfDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServiceObservation>().HasKey(so => so.Id);
        modelBuilder.Entity<PinnedService>().HasKey(ps => ps.ServiceName);
    }

    public DbSet<PinnedService> PinnedServices { get; set; }
    public DbSet<ServiceObservation> ServiceObservations { get; set; }
}

public class ServiceObservation
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public required string ServiceName { get; set; }
    public required string ServiceStatus { get; set; }
}

public class PinnedService
{
    public required string ServiceName { get; set; }
}
