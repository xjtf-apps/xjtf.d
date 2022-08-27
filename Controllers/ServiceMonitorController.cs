using xjtf.d.dto;

namespace xjtf.d;

public class ServiceMonitorController
{
    private readonly DaemonDbContext _dbContext;

    public ServiceMonitorController(DaemonDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [Route("/Monitor/Services")][HttpGet]
    public IActionResult GetServices() // : ServicesInfoPayload
    {
        var services = ServiceMonitor.GetServiceStatuses().ToList();
        return new JsonResult(new ServicesInfoPayload() { Services = services });
    }

    //[Route("/Monitor/Service/{serviceId}")][HttpGet]
    //public IActionResult GetService(int serviceId) // : ServiceDetailsPayload
    //{

    //}

    //[Route("/Monitor/Service/{serviceName}")][HttpGet]
    //public IActionResult GetService(string serviceName) // : ServiceDetailsPayload
    //{

    //}

    //[Route("/Monitor/Service")][HttpPost]
    //public IActionResult UpsertServiceEntry([FromBody] ServiceEntryPayload payload)
    //{

    //}

    //[Route("/Monitor/Service/State/{serviceId}")][HttpPost]
    //public IActionResult InsertServiceEntryState([FromBody] ServiceEntryStatePayload payload, int serviceId)
    //{

    //}

    //[Route("/Monitor/Service/State/{serviceName}")][HttpPost]
    //public IActionResult InsertServiceEntryState([FromBody] ServiceEntryStatePayload payload, string serviceName)
    //{

    //}

    //[Route("/Monitor/Clutch")][HttpPut]
    //public IActionResult SetClutch([FromBody] DaemonDettachStatePayload payload)
    //{

    //}

    //[Route("/Monitor/Services/Subscribe")][HttpGet]
    //public IActionResult GetServicesSubscription() // : ServicesInfoPayload
    //{

    //}

    //[Route("/Monitor/Service/Subscribe/{serviceId}")][HttpGet]
    //public IActionResult GetServiceSubscription(int serviceId) // : ServiceDetailsPayload
    //{

    //}

    //[Route("/Monitor/Service/Subscribe/{serviceName}")][HttpGet]
    //public IActionResult GetServiceSubscription(string serviceName) // : ServiceDetailsPayload
    //{

    //}
}