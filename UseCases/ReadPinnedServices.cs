using Microsoft.EntityFrameworkCore;

namespace xjtf.d.ui._2;

public class ReadPinnedServices
{
    public PinnedServicesDto GetPinnedServices(XjtfDbContext context)
    {
        return new PinnedServicesDto
        {
            PinnedServices = context.PinnedServices.AsNoTracking().Select(ps => ps.ServiceName).ToArray()
        };
    }
}
