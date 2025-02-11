using Microsoft.AspNetCore.Mvc;

namespace xjtf.d.ui._2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PinnedServicesController : ControllerBase
{
    private readonly XjtfDbContext _context;
    private readonly ILogger<PinnedServicesController> _logger;
    private readonly Lazy<SetPinnedService> _setPinnedService = new();
    private readonly Lazy<UnsetPinnedService> _unsetPinnedService = new();
    private readonly Lazy<ReadPinnedServices> _readPinnedServices = new();

    public PinnedServicesController(ILogger<PinnedServicesController> logger, XjtfDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public PinnedServicesDto GetPinnedServices()
    {
        return _readPinnedServices.Value.GetPinnedServices(_context);
    }

    [HttpPut]
    [Route("pin/{serviceName}")]
    public void SetPinnedService(string serviceName)
    {
        _setPinnedService.Value.Set(_context, serviceName);
        _logger.LogInformation($"Service {serviceName} pinned.");
    }

    [HttpPut]
    [Route("unpin/{serviceName}")]
    public void UnsetPinnedService(string serviceName)
    {
        _unsetPinnedService.Value.Unset(_context, serviceName);
        _logger.LogInformation($"Service {serviceName} unpinned.");
    }
}
