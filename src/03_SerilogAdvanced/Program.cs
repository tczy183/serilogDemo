#define CustomProperty
#define ProcessId
#define SourceContext
#define ThreadId
#define EnvironmentName
#define EnvironmentUserName
#define Filter

using Serilog;
using Serilog.Configuration;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;

var outputTemplate = CreateTemplate();
// 初始化 Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)

    #region Enricher

#if SourceContext
    .Enrich.FromLogContext()
#endif
#if ThreadId
    .Enrich.WithThreadId()
#endif
#if ProcessId
    .Enrich.WithProcessId()
#endif
#if EnvironmentName
    .Enrich.WithEnvironmentName()
#endif
#if EnvironmentUserName
    .Enrich.WithEnvironmentUserName()
#endif
#if CustomProperty
    // .Enrich.With<CustomPropertyEnricher>()
    .Enrich.WithCustomProperty()
#endif

    #endregion

    #region Async

#if !Filter
    .WriteTo.Async(a => a.Console(outputTemplate: outputTemplate))
    .WriteTo.Async(a => a.File("Logs/logs-.txt", LogEventLevel.Verbose, rollingInterval: RollingInterval.Day,
        outputTemplate: outputTemplate))
    .WriteTo.Async(a => a.SQLite("Logs/log.db"))
#endif

    #endregion

    #region Filter

#if Filter
    .WriteTo.Logger(a =>
        a.Filter.ByIncludingOnly(b =>
                b.Properties.ContainsKey("CustomProperty") &&
                Convert.ToInt32(b.Properties["CustomProperty"].ToString()) > 5)
            .WriteTo.Async(c => c.Console(outputTemplate: outputTemplate))
    )
    .WriteTo.Logger(a =>
        a.Filter.ByIncludingOnly(b => b.Level == LogEventLevel.Verbose)
            .WriteTo.Async(c => c.File("Logs/Verbose/logs-.txt", rollingInterval: RollingInterval.Day,
                outputTemplate: outputTemplate))
    )
    .WriteTo.Logger(a =>
        a.Filter.ByIncludingOnly(b => b.Level == LogEventLevel.Debug)
            .WriteTo.Async(c => c.File("Logs/Debug/logs-.txt", rollingInterval: RollingInterval.Day,
                outputTemplate: outputTemplate))
    )
    .WriteTo.Logger(a =>
        a.Filter.ByIncludingOnly(b => b.Level == LogEventLevel.Information)
            .WriteTo.Async(c => c.File("Logs/Information/logs-.txt", rollingInterval: RollingInterval.Day,
                outputTemplate: outputTemplate))
    )
    .WriteTo.Logger(a =>
        a.Filter.ByIncludingOnly(b => b.Level == LogEventLevel.Warning)
            .WriteTo.Async(c => c.File("Logs/Warning/logs-.txt", rollingInterval: RollingInterval.Day,
                outputTemplate: outputTemplate))
    )
    .WriteTo.Logger(a =>
        a.Filter.ByIncludingOnly(b => b.Level == LogEventLevel.Error)
            .WriteTo.Async(c => c.File("Logs/Error/logs-.txt", rollingInterval: RollingInterval.Day,
                outputTemplate: outputTemplate))
    )
    .WriteTo.Logger(a =>
        a.Filter.ByIncludingOnly(b => b.Level == LogEventLevel.Fatal)
            .WriteTo.Async(c => c.File("Logs/Fatal/logs-.txt", rollingInterval: RollingInterval.Day,
                outputTemplate: outputTemplate))
    )
    
#endif

    #endregion

    .CreateLogger();

try
{
    Log.ForContext<Program>().Verbose("This is a verbose message");
    Log.ForContext<Program>().Debug("This is a debug message");
    Log.ForContext<Program>().Information("This is an information message");
    Log.ForContext<Program>().Warning("This is a warning message");
    Log.ForContext<Program>().Error("This is an error message");
    Log.ForContext<Program>().Fatal("This is a fatal message");

    await Task.Run(() => { Log.ForContext<Program>().Verbose("This is a verbose message"); });
}
catch (Exception ex)
{
    Log.ForContext<Program>().Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static string CreateTemplate()
{
    var result = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}]  ";

#if CustomProperty
    result += "[{CustomProperty}] ";
#endif

#if ProcessId
    result += "[{ProcessId}] ";
#endif

#if SourceContext
    result += "[{SourceContext}] ";
#endif

#if ThreadId
    result += "[{ThreadId}] ";
#endif

#if EnvironmentName
    result += "[{EnvironmentName}] ";
#endif

#if EnvironmentUserName
    result += "[{EnvironmentUserName}] ";
#endif

    result += "- {Message:lj}{NewLine}{Exception}";
    return result;
}

//添加一个自定义ILogEventEnricher
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