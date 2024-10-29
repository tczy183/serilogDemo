using Serilog;
using Serilog.Core;

var tepmlate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u4}] - {Message:lj}{NewLine}{Exception}";
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console(outputTemplate: tepmlate)
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, outputTemplate: tepmlate)
    .WriteTo.SQLite("logs/log.db")
    .CreateLogger();

Log.Verbose("This is a verbose message");
Log.Debug("This is a debug message");
Log.Information("This is an information message");
Log.Warning("This is a warning message");
Log.Error("This is an error message");
Log.Fatal("This is a fatal message");

//结构化
Log.Information("This is a message with {Property1} and {Property2}", 1, 2);
var person = new Person("Doe", "John", 25);
Log.Information("This is a person {Person}", person);
Log.Information("This is a person {@Person}", person);
var persons = new List<Person>
{
    new("Doe", "John", 25),
    new("Sed", "Jane", 22)
};
Log.Information("These are persons {Persons}", persons);
Log.Information("These are persons {@Persons}", persons);


Log.CloseAndFlush();

public record Person(string FirstName, string LastName, int Age);