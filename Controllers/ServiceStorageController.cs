namespace xjtf.d;

[Authorize][EnableCors]
public class ServiceStorageController : BaseController
{
    private readonly ServiceStore _serviceStore;

    public ServiceStorageController
    (
        ServiceStore serviceStore
    )
    {
        _serviceStore = serviceStore;
    }

    [Route("/Store/Browse")][HttpGet]
    public IActionResult Browse()
    {
        return new JsonResult(_serviceStore.EnumerateStores());
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
        var route = $"/Store/Browse/{serviceName}";
        AuditServiceUpload(route, serviceName);
        return Created(route, serviceName);
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
            AuditServiceDelete(serviceName);
            store.Clear();
            return Ok();
        }
        else return BadRequest(); 
    }

    #region audit methods
    private void AuditServiceUpload(string route, string serviceName)
    {
        Auditor.LogEntry(AuditLogWriter, new AuditRecord()
        {
            Subject = AuditSubjectUser,
            Verb = AuditVerbs.Storage.ServiceUploaded,
            Object = $"Entity({serviceName}), HttpResource({route})",
            EventKey = AuditEventKeys.Area.Store
        });
    }

    private void AuditServiceDelete(string serviceName)
    {
        Auditor.LogEntry(AuditLogWriter, new AuditRecord()
        {
            Subject = AuditSubjectUser,
            Verb = AuditVerbs.Storage.ServiceDeleted,
            Object = $"Entity({serviceName})",
            EventKey = AuditEventKeys.Area.Store
        });
    }
    #endregion
}