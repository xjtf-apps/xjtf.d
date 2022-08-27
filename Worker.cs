// using xjtf.threads;
// using xjtf.threads.green;
// using System.ServiceProcess;
// using System.Management.Automation;
// using System.Collections.ObjectModel;

// namespace xjtf.d;

// public class Worker : BackgroundService, IDisposable
// {
//     private static GreenThread? _thread;
//     private readonly dynamic _config;
//     private static ILogger<Worker>? _logger;
//     private static ServiceStatusPublisher? _publisher;
//     private static readonly GreenThreadScheduler _scheduler = GreenThreadScheduler.Default;

//     public Worker(Init _, dynamic config, ServiceStatusPublisher publisher, ILogger<Worker> logger)
//     {
//         _config = config;
//         _logger = logger;
//         _publisher = publisher;
//     }

//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         await Task.Run(async () =>
//         {
//             _thread = new(new GreenThreadStart(Work));

//             _thread.Configure(_config);
//             _scheduler.Schedule(_thread);
//             var scheduler = (IThreadScheduler<GreenThread>)_scheduler;
//             using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

//             while (!cts.IsCancellationRequested)
//             {
//                 await scheduler.RunOnceAsync();
//                 await Task.Delay(1_000); // ms
//             }
//         },
//         CancellationToken.None);
//     }

//     private static void Work()
//     {
//         var configuration = (dynamic)_thread!.Configuration!;

//         if (ServicesAreDefined(configuration, out IList<ServiceStartInfo>? serviceInfos))
//         {
//             EnsureInstalled(serviceInfos!);
//             EnsureStarted(serviceInfos!);
//         }
//         _publisher?.UpdateStatus();

//         // reschedule self
//         _thread = _thread.Reschedule(_scheduler);
//     }

//     private static void EnsureInstalled(IList<ServiceStartInfo> serviceInfos)
//     {
//         var systemServices = ServiceController.GetServices();
//         var systemServiceNames = systemServices.Select(ss => ss.ServiceName);
//         var requiredServiceNames = serviceInfos.Select(ssi => ssi.ServiceName);
//         var missingServiceNames = requiredServiceNames.Where(rsn => !systemServiceNames.Contains(rsn));

//         var missingServices = serviceInfos
//             .IntersectBy(missingServiceNames, i => i.ServiceName);

//         foreach (var missingService in missingServices)
//         {
//             try
//             {
//                 var serviceName = missingService.ServiceName;
//                 var binaryPath = $"{missingService.Executable} {missingService.Arguments}";
//                 var script = $"New-Service -Name '{serviceName}' -BinaryPath '{binaryPath}' -StartupType \"Automatic\"";

//                 _logger?.LogInformation(message: "Running script: {Script}", script);

//                 using var shell = PowerShell.Create();
//                 shell.AddScript(script);
//                 Print(shell.Invoke());
//             }
//             catch (Exception e)
//             {
//                 _logger?.LogError(e, message: "Error installing missing service: {ServiceName}", missingService.ServiceName);
//             }
//         }
//     }

//     private static void EnsureStarted(IList<ServiceStartInfo> serviceInfos)
//     {
//         var systemServices = ServiceController.GetServices();
//         var systemServiceNames = systemServices.Select(ss => ss.ServiceName);
//         var requiredServiceNames = serviceInfos.Select(ssi => ssi.ServiceName);
//         var installedServiceNames = requiredServiceNames.Where(rsn => systemServiceNames.Contains(rsn));

//         var stoppedServices = systemServices
//             .IntersectBy(installedServiceNames, ss => ss.ServiceName)
//             .Where(ss => ss.Status == ServiceControllerStatus.Stopped);

//         foreach (var stoppedService in stoppedServices)
//         {
//             try
//             {
//                 _logger?.LogInformation(message: "Starting service: {ServiceName}", stoppedService.ServiceName);

//                 var ssi = serviceInfos.First(i => i.ServiceName == stoppedService.ServiceName);
//                 var args = ssi.Arguments.Split(' ');
//                 stoppedService.Start(args);
//             }
//             catch (Exception e)
//             {
//                 _logger?.LogError(e, message: "Error starting requested service: {ServiceName}", stoppedService.ServiceName);
//             }
//         }
//     }

//     private static bool ServicesAreDefined(dynamic config, out IList<ServiceStartInfo>? serviceInfos)
//     {
//         serviceInfos = null;

//         if (!config.Exists) return false;

//         using var stream = File.OpenRead(config.Path);
//         using var reader = new StreamReader(stream);
//         var contents = reader.ReadToEnd();

//         serviceInfos = contents
//             .Split(Environment.NewLine)
//             .Select(line => new ServiceStartInfo(line)).ToList();

//         return serviceInfos.Count > 0;
//     }

//     private static void Print(Collection<PSObject> psObjects)
//     {
//         if (psObjects.Count == 0)
//             _logger?.LogError("Failed to install service");
//         else
//             _logger?.LogWarning("Successfully installed service");
//     }

//     new public void Dispose()
//     {
//         _thread?.Dispose();
//         base.Dispose();
//     }
// }
