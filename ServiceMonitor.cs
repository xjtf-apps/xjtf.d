using System;
using System.Threading;
using System.Management;
using System.ServiceProcess;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ServiceMonitor : BackgroundService
{
    private readonly ILogger<ServiceMonitor> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ServiceMonitor(ILogger<ServiceMonitor> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ServiceMonitor is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<XjtfDbContext>();

            foreach (var service in ServiceController.GetServices())
            {
                var timestamp = DateTime.Now;
                var serviceName = service.ServiceName;
                var serviceStatus = service.Status.ToString();

                var record = new ServiceObservation
                {
                    Timestamp = timestamp,
                    ServiceName = serviceName,
                    ServiceStatus = serviceStatus
                };
                dbContext.ServiceObservations.Add(record);
            }
            dbContext.SaveChanges();
            _logger.LogInformation("ServiceMonitor added new records.");

            var outdatedRecords = dbContext.ServiceObservations.Where(o => o.Timestamp < DateTime.Now.AddDays(-7)).ToList();
            dbContext.ServiceObservations.RemoveRange(outdatedRecords);
            dbContext.SaveChanges();
            _logger.LogInformation($"ServiceMonitor removed {outdatedRecords.Count} outdated records.");

            await Task.Delay(5000, stoppingToken);
        }

        _logger.LogInformation("ServiceMonitor is stopping.");
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ServiceMonitor is stopping.");
        await base.StopAsync(stoppingToken);
    }
}
