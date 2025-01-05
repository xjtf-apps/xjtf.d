using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseWindowsService(opt => opt.ServiceName = "Xjtf Daemon");
builder.Services.AddControllers();
builder.Services.AddWindowsService();
builder.Services.AddHostedService<ServiceMonitor>();
builder.Services.AddDbContext<XjtfDbContext>(options => {
    options.UseSqlite("Data Source=xjtf.db");
});
var app = builder.Build();

// Fallback to `index.html` for React Router
app.Use(async (context, next) =>
{
    if (context.Request.Path.Value!.StartsWith("/api"))
    {
        await next();
        return;
    }

    if (context.Request.Path.Value == "/")
    {
        context.Request.Path = "/index.html";
        await next();
        return;
    }

    await next();
});

// Serve static files from the embedded resource store
app.UseMiddleware<EmbeddedResourceMiddleware>();

// Serve static files from the React build folder
// var reactAppPath = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp", "dist");
// app.UseStaticFiles(new StaticFileOptions
// {
//     FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(reactAppPath),
//     RequestPath = ""
// });

// Map API endpoints
app.MapControllers();

app.Run();
