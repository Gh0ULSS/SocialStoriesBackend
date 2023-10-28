using Serilog;

namespace SocialStoriesBackend;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true) // Development
            .Build();

        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        // setup logger
        Log.Logger = logger;

        try
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host
            .CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}