namespace xjtf.d.ui._2;

public static class EnsureCreated
{
    public static void DatabaseReady(WebApplication? app)
    {
        using var scope = app!.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<XjtfDbContext>();
        dbContext.Database.EnsureCreated();
    }
}