using Microsoft.AspNetCore.Mvc;

namespace xjtf.d.ui._2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WindowsServicesController : ControllerBase
{
    private readonly XjtfDbContext _context;
    private readonly ReadServices _readServices = new();
    private readonly ReadServiceWithUptime _readServiceWithUptime = new();

    public WindowsServicesController(XjtfDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IEnumerable<WindowsServiceDto> Get()
    {
        return _readServices.GetServices();
    }

    [HttpGet]
    [Route("{serviceName}")]
    public WindowsServiceWithUptimeDto? Get(string serviceName)
    {
        return _readServiceWithUptime.GetService(serviceName, _context, DateTime.Now);
    }
}
