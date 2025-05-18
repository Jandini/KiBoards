using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Serilog;
using KiBoards.Management;

internal static class Extensions
{
    internal static void LogVersion<T>(this IServiceProvider provider) => provider
        .GetRequiredService<ILogger<T>>()
        .LogInformation($"KiBoards.Management.Cli {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");

    internal static CancellationTokenSource GetCancellationTokenSource(this IServiceProvider provider)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                provider.GetRequiredService<ILogger<Program>>()
                    .LogWarning("User break (Ctrl+C) detected. Shutting down gracefully...");

                cancellationTokenSource.Cancel();
                eventArgs.Cancel = true;
            }
        };

        return cancellationTokenSource;
    }

    internal static IConfigurationBuilder AddApplicationSettings(this IConfigurationBuilder builder)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        return builder
            .AddEmbeddedJsonFile("appsettings.json")
            .AddEmbeddedJsonFile($"appsettings.{environment}.json");
    }

    internal static IConfigurationBuilder AddEmbeddedJsonFile(this IConfigurationBuilder builder, string name)
    {
        var fileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        var fileInfo = fileProvider.GetFileInfo(name);

        if (fileInfo.Exists)
            builder.AddJsonStream(fileInfo.CreateReadStream());

        return builder.AddJsonFile(name, true);
    }

    internal static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddSingleton(configuration);
    }

    internal static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddLogging(builder => builder
            .AddSerilog(new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger(), dispose: true));
    }

    internal static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddHttpClient<KibanaHttpClient>(client => client.BaseAddress = new Uri("http://localhost:5601"));

        return services
            // Add services here
            .AddTransient<Main>();
    }
}
