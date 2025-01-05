using System.Reflection;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

public class EmbeddedResourceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IFileProvider _fileProvider;

    public EmbeddedResourceMiddleware(RequestDelegate next)
    {
        _next = next;
        // Create a file provider for embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        _fileProvider = new ManifestEmbeddedFileProvider(assembly, "ClientApp/dist");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.TrimStart('/');

        if (!string.IsNullOrEmpty(path))
        {
            var fileInfo = _fileProvider.GetFileInfo(path);

            if (fileInfo.Exists)
            {
                var contentType = GetContentType(path);
                context.Response.ContentType = contentType;

                using var stream = fileInfo.CreateReadStream();
                await stream.CopyToAsync(context.Response.Body);
                return;
            }
        }

        // Call the next middleware if no embedded resource is found
        await _next(context);
    }

    private static string GetContentType(string path)
    {
        var provider = new FileExtensionContentTypeProvider();
        return provider.TryGetContentType(path, out var contentType) ? contentType : "application/octet-stream";
    }
}
