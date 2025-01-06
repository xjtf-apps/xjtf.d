using System.ServiceProcess;

namespace xjtf.d.ui._2;

public class ReadServices
{
    public WindowsServiceDto[] GetServices()
    {
        return ServiceController.GetServices().Select(s => new WindowsServiceDto
        {
            ServiceName = s.ServiceName,
            DisplayName = s.DisplayName,
            Status = s.Status.ToString(),
            Description = string.Empty,
            StartType = s.StartType.ToString()
            
        }).ToArray();
    }
}