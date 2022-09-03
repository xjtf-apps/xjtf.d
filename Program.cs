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
            builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<DaemonDbContext>();
            builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtOptions);
            builder.Services.AddLogging(builder => builder.AddSeq(SeqConfig));
            builder.Services.AddMvc(opt => opt.EnableEndpointRouting = false);
            builder.Services.AddSingleton<CommandRunnerRestAdapterFactory>();
            builder.Services.AddTransient<ServiceTimeseriesAggregator>();
            builder.Services.AddTransient<CommandRunnerRestAdapter>();
            builder.Services.AddSingleton<CommandRunnerFactory>();
            builder.Services.AddSingleton<MonitorDataService>();
            builder.Services.AddTransient<GetServicesWorker>();
            builder.Services.AddHostedService<MonitorWorker>();
            builder.Services.AddTransient<GetServiceWorker>();
            builder.Services.AddSingleton<DaemonDbContext>();
            builder.Services.AddSingleton<MonitorResults>();
            builder.Services.AddTransient<ServiceStore>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSingleton(Restartable);
            builder.Services.AddSingleton<Init>();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseWebSockets();
            app.UseMvc();
            return app;
        };
        await appFactory.RunRestartableAsync(Restartable);
    }

    private static IConfiguration Configuration
    {
        get
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }
    }

    private static IConfigurationSection SeqConfig
    {
        get
        {
            return Configuration.GetSection("Seq");
        }
    }

    private static IConfigurationSection JwtConfig
    {
        get
        {
            return Configuration.GetSection("Jwt");
        }
    }

    internal static string JwtAudience => JwtConfig.GetValue<string>("Audience")!;
    internal static string JwtIssuer => JwtConfig.GetValue<string>("Issuer")!;
    internal static string JwtKey => JwtConfig.GetValue<string>("Key")!;

    private static Action<JwtBearerOptions> JwtOptions = (opt) =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = JwtIssuer,
            ValidAudience = JwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true
        };
    };
}
