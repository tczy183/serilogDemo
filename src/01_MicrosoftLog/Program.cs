using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole(); // 输出日志到控制台
});

var logger = loggerFactory.CreateLogger<Program>();

// Use the logger
logger.LogTrace("This is a trace log");
logger.LogDebug("This is a debug log");
logger.LogInformation("This is an information log");
logger.LogWarning("This is a warning log");
logger.LogError("This is an error log");
logger.LogCritical("This is a critical log");

logger.Log(LogLevel.Information, 1,"This is a log message");

var person = new Person("Doe", "John", 25);
logger.LogInformation("This is a person {Person}", person);
logger.LogInformation("This is a person {@Person}", person);

public record Person(string FirstName, string LastName, int Age);