namespace xjtf.d;

public class BaseController : ControllerBase
{
    private protected AuditLogWriter AuditLogWriter = AuditLogWriter.Instance;

    private protected string AuditSubjectUser
    {
        get
        {
            var user = HttpContext.User;
            var userSubject = user?.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"))?.Value;
            var userActionSource = userSubject != null ? $"Username({userSubject})" : "Anonymous";

            return userActionSource;
        }
    }
}
