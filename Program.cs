namespace xjtf.d;

public class Program
{
    public static readonly Restartable Restartable = new();
    public static IHostApplicationLifetime? ApplicationLifetime = null;

    public static async Task Main(string[] args)
    {
        var appFactory = () =>
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseWindowsService(opt => opt.ServiceName = "xjtf daemon");
            builder.Services.AddDataProtection(opt => opt.ApplicationDiscriminator = "xjtf.d");
            builder.Services.AddLogging(builder => builder.AddSeq(SeqConfig));
            builder.Services.AddMvc(opt => opt.EnableEndpointRouting = false);
            builder.Services.AddHostedService<ServiceMonitor>();
            // builder.Services.AddHostedService<Worker>();
            builder.Services.AddSingleton<DaemonDbContext>();
            builder.Services.AddSingleton(Restartable);
            builder.Services.AddSingleton<Init>();

            var app = builder.Build();
            app.UseMvc();
            return app;
        };
        await appFactory.RunRestartableAsync(Restartable);
    }

    private static IConfiguration Configuration
    {
        get
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }
    }

    private static IConfigurationSection SeqConfig
    {
        get
        {
            return Configuration.GetSection("Seq");
        }
    }
}
