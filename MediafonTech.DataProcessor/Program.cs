using MediafonTech.DataProcessor;
using MediafonTech.Infrastructure;
using Serilog;
using Serilog.Events;

// Build configuration to read data from appsettings.json
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .Build();

// Created Serilog logger Configuration
Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

// Added a global exception handlding.
try
{
    Log.Information("Application Starting up");

    IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<DataProcessor>()
                .RegisterInfrastructure(hostContext.Configuration);
    })
    .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}


