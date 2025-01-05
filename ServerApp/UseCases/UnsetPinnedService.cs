namespace xjtf.d.ui._2;

public class UnsetPinnedService
{
    public void Unset(XjtfDbContext context, string serviceName)
    {
        var pinnedService = context.PinnedServices.FirstOrDefault(ps => ps.ServiceName == serviceName);
        if (pinnedService != null)
        {
            context.PinnedServices.Remove(pinnedService);
            context.SaveChanges();
        }
    }
}