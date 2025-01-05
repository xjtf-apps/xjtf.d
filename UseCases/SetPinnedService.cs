namespace xjtf.d.ui._2;

public class SetPinnedService
{
    public void Set(XjtfDbContext context, string serviceName)
    {
        context.PinnedServices.Add(new PinnedService { ServiceName = serviceName });
        context.SaveChanges();
    }
}
