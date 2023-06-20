using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

// Configure and create HttpClient mock
var builder = WebApplication.CreateBuilder();
builder.Logging.ClearProviders();
builder.WebHost.UseKestrel();
await using var application = builder.Build();

application.MapGet("/", async context =>
{
    await context.Response.WriteAsync(new string('-', 20));
    await context.Response.WriteAsync("\n");
    foreach (var (k, v) in context.Request.Headers)
        await context.Response.WriteAsync($"{k}: {v}\n");
});

await application.RunAsync();