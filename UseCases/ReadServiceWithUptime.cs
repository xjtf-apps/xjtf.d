using System.Management;
using System.ServiceProcess;

namespace xjtf.d.ui._2;

public class ReadServiceWithUptime
{
    public WindowsServiceWithUptimeDto? GetService(string serviceName, XjtfDbContext context, DateTime currentTime)
    {   
        #if WINDOWS
        var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName);
        if (service == null) return null;

        return new WindowsServiceWithUptimeDto
        {
            ServiceName = service.ServiceName,
            DisplayName = service.DisplayName,
            Status = service.Status.ToString(),
            Description = GetServiceDescription(serviceName),
            StartType = service.StartType.ToString(),
            Uptime = GetServiceUptime(serviceName, context, currentTime),
            DetailedUptime = GetServiceDetailedUptime(serviceName, context, currentTime)
        };
        #else
        return null;
        #endif
    }

    private UptimeDto[] GetServiceDetailedUptime(string serviceName, XjtfDbContext context, DateTime currentTime)
    {
        // Filter observations for the specific service
        var serviceObservations = context.ServiceObservations
            .Where(o => o.ServiceName == serviceName)
            .Where(o => o.Timestamp > currentTime.AddMinutes(-30))
            .OrderBy(o => o.Timestamp)
            .ToList();

        return serviceObservations.Select(o => new UptimeDto
        {
            Timestamp = o.Timestamp.ToString("YYYY-MM-DDTHH:mm:ss.sssZ"),
            Status = o.ServiceStatus,
            Duration = 0

        }).ToArray();
    }

    private UptimeDto[] GetServiceUptime(string serviceName, XjtfDbContext context, DateTime currentTime)
    {
        // Filter observations for the specific service
        var serviceObservations = context.ServiceObservations
            .Where(o => o.ServiceName == serviceName)
            .OrderBy(o => o.Timestamp)
            .ToList();

        // Group observations by day
        var groupedObservations = serviceObservations
            .GroupBy(o => o.Timestamp.Date);

        var dailyRecords = new List<UptimeDto>();

        foreach (var group in groupedObservations)
        {
            var day = group.Key; // The day of the group
            var dailyObservations = group.ToList();

            DateTime? runningStart = null;

            for (int i = 0; i < dailyObservations.Count; i++)
            {
                var observation = dailyObservations[i];

                if (observation.ServiceStatus == "Running")
                {
                    // Mark the start of a "Running" period
                    if (runningStart == null)
                    {
                        runningStart = observation.Timestamp;
                    }
                }
                else if (observation.ServiceStatus == "Stopped" && runningStart != null)
                {
                    // Calculate duration for the "Running" period up to this "Stopped" status
                    var duration = (observation.Timestamp - runningStart.Value).TotalMinutes;
                    dailyRecords.Add(new UptimeDto
                    {
                        Timestamp = day.ToString("yyyy-MM-dd"),
                        Status = "Running",
                        Duration = (int)Math.Round(duration)
                    });

                    runningStart = null; // Reset the start time
                }
            }

            // Handle the case where the service was running until the end of the day
            if (runningStart.HasValue)
            {
                var endOfDayOrNow = day == currentTime.Date
                    ? currentTime
                    : day.AddDays(1).AddTicks(-1); // Last moment of the day or current time

                var duration = (endOfDayOrNow - runningStart.Value).TotalMinutes;
                dailyRecords.Add(new UptimeDto
                {
                    Timestamp = day.ToString("yyyy-MM-dd"),
                    Status = "Running",
                    Duration = (int)Math.Round(duration)
                });
            }
        }

        // Sum durations for the same day
        var aggregatedResults = dailyRecords
            .GroupBy(record => record.Timestamp)
            .Select(group => new UptimeDto
            {
                Timestamp = group.Key,
                Status = "Running",
                Duration = group.Sum(record => record.Duration)
            })
            .ToList();

        return aggregatedResults.ToArray();
    }

    private string GetServiceDescription(string serviceName)
    {
        #if WINDOWS
        try
        {
            // Query WMI for the service description
            var query = $"SELECT Description FROM Win32_Service WHERE Name = '{serviceName}'";
            using (var searcher = new ManagementObjectSearcher(query))
            using (var collection = searcher.Get())
            {
                foreach (var obj in collection)
                {
                    return obj["Description"]?.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching description for {serviceName}: {ex.Message}");
        }
        #endif
        return string.Empty;
    }
}