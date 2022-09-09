namespace xjtf.d;

[Authorize][EnableCors]
public class ServiceControlController : BaseController
{
    private readonly DaemonDbContext _dbContext;

    public ServiceControlController
    (
        DaemonDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    [Route("/Control/Services")][HttpGet]
    public async Task<IActionResult> GetControlledServices()
    {
        var serviceControls = (await _dbContext
            .ServiceControls.Where(svc => svc.Deleted == null).ToListAsync())
            .OrderBy(svc => svc.ServiceName);

        return new JsonResult(serviceControls.ToArray());
    }

    [Route("/Control/Service/{serviceName}")][HttpGet]
    public async Task<IActionResult> GetControlledService(string serviceName)
    {
        var serviceControl = (await _dbContext
            .ServiceControls.Where(svc => svc.ServiceName == serviceName).ToListAsync())
            .OrderByDescending(svc => svc.Created)
            .Where(svc => svc.Deleted == null)
            .FirstOrDefault();

        return new JsonResult(serviceControl);
    }

    [Route("/Control/Service/{serviceName}")][HttpDelete]
    public async Task<IActionResult> DeleteControlledService(string serviceName)
    {
        // TODO: audit

        var serviceControl = (await _dbContext
            .ServiceControls.Where(svc => svc.ServiceName == serviceName).ToListAsync())
            .OrderByDescending(svc => svc.Created)
            .Where(svc => svc.Deleted == null)
            .FirstOrDefault();

        if (serviceControl != null)
        {
            serviceControl.Deleted = DateTimeOffset.Now;
            await _dbContext.SaveChangesAsync();
        }
        return new JsonResult(new { deleted = serviceControl != null });
    }

    [Route("/Control/Service/{serviceName}")][HttpPost]
    public async Task<IActionResult> CreateControlledService(string serviceName)
    {
        var previousServiceControl = (await _dbContext
            .ServiceControls.Where(svc => svc.ServiceName == serviceName).ToListAsync())
            .OrderByDescending(svc => svc.Created)
            .Where(svc => svc.Deleted == null)
            .FirstOrDefault();

        if (previousServiceControl != null)
            return BadRequest(previousServiceControl);

        // TODO: identify if service is daemon managed

        var control = new ServiceControl()
        {
            ServiceName = serviceName,
            StoreBased = false,
            Created = DateTimeOffset.Now,
            Deleted = null
        };
        _dbContext.ServiceControls.Add(control);
        await _dbContext.SaveChangesAsync();
        return new JsonResult(control);
    }
}
