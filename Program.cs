using Swashbuckle.AspNetCore.SwaggerGen;

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
            builder.Services.AddTransient<DaemonDbContext>(provider =>
            {
                var factory = provider.GetRequiredService<DaemonDbContextFactory>();
                var instance = factory.GetNew();
                return instance;
            });
            builder.Services.AddLogging(builder => builder.AddSeq(SeqConfig));
            builder.Services.AddMvc(opt => opt.EnableEndpointRouting = false);
            builder.Services.AddTransient<ServiceTimeseriesAggregator>();
            builder.Services.AddTransient<CommandRunnerRestAdapter>();
            builder.Services.AddTransient<ServiceStatsTransformer>();
            builder.Services.AddSingleton<DaemonDbContextFactory>();
            builder.Services.AddSingleton<CommandRunnerFactory>();
            builder.Services.AddTransient<GetServicesWorker>();
            builder.Services.AddHostedService<MonitorWorker>();
            builder.Services.AddSwaggerGen(SwaggerGenOptions);
            builder.Services.AddTransient<GetServiceWorker>();
            builder.Services.AddSingleton<MonitorService>();
            builder.Services.AddTransient<CommandRunner>();
            builder.Services.AddCors(opt =>
            {
                opt.AddDefaultPolicy(builder => 
                {
                    builder.WithOrigins("http://localhost:3000");
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
            });
            builder.Services.AddSingleton<MonitorResults>();
            builder.Services.AddSingleton<MonitorClutch>();
            builder.Services.AddTransient<ServiceStore>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSingleton(Restartable);
            builder.Services.AddSingleton<Init>();

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
            app.UseCors();
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

    private static Action<SwaggerGenOptions> SwaggerGenOptions = (opt) =>
    {
        opt.SwaggerDoc("v1", new OpenApiInfo { Title = "xjtf.d API", Version = "v1" });
        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            Description = "Please enter a valid token"
        });
        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
    };
}
