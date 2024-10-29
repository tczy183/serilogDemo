 #define Filter

using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

var configuration = new ConfigurationBuilder()
#if Filter
    .AddJsonFile("appsettings.filter.json")
#else
     .AddJsonFile("appsettings.json")
#endif

    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

var forContext = Log.ForContext<Program>();
try
{
    forContext.Verbose("This is a verbose message");
    forContext.Debug("This is a debug message");
    forContext.Information("This is an information message");
    forContext.Warning("This is a warning message");
    forContext.Error("This is an error message");
    forContext.Fatal("This is a fatal message");
}
catch (Exception ex)
{
    forContext.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public class CustomPropertyEnricher : ILogEventEnricher
{
    const string CustomPropertyName = "CustomProperty";

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var next = Random.Shared.Next(0, 10);

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(CustomPropertyName, next));
    }
}

public static class CustomPropertyEnricherExtensions
{
    public static LoggerConfiguration WithCustomProperty(this LoggerEnrichmentConfiguration enrich)
    {
        return enrich.With<CustomPropertyEnricher>();
    }
}