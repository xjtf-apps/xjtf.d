namespace xjtf.d;

[Authorize][EnableCors]
public class ServiceStorageController : ControllerBase
{
    private readonly ServiceStore _serviceStore;

    public ServiceStorageController
    (
        ServiceStore serviceStore,
        CommandRunnerRestAdapter commandRunner
    )
    {
        _serviceStore = serviceStore;
    }

    [Route("/Store/Service/Upload/{serviceName}")][HttpPost]
    public async Task<IActionResult> UploadServiceAsync(List<IFormFile> serviceFiles, string serviceName)
    {
        var store = _serviceStore.AccessStore(serviceName);
        if (store.Empty)
        {
            await store.AddFormFilesAsync(serviceFiles);
        }
        else return BadRequest();

        return Created($"/Store/Browse/{serviceName}", serviceName);
    }

    [Route("/Store/Service/Browse/{serviceName}")][HttpGet]
    public IActionResult BrowseService(string serviceName)
    {
        var store = _serviceStore.AccessStore(serviceName);
        if (store.Empty)
        {
            return NoContent();
        }
        else return new JsonResult(store.EnumerateContents().ToArray());
    }

    [Route("/Store/Service/Delete/{serviceName}")][HttpDelete]
    public IActionResult DeleteService(string serviceName)
    {
        var store = _serviceStore.AccessStore(serviceName);
        if (!store.Empty)
        {
            // TODO: stop service, detach monitor
            store.Clear();
            return Ok();
        }
        else return BadRequest(); 
    }
}