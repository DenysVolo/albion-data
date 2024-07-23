using service;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose() 
    .WriteTo.Console() 
    .WriteTo.File("logs/service-errors.log", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error) 
    .CreateLogger();

builder.Services.AddSerilog(); 

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddSingleton<IDatabaseHandler, DatabaseHandler>();
builder.Services.AddHostedService<MarketOrderListener>();
builder.Services.AddHostedService<MarketHistoryListener>();

var host = builder.Build();
host.Run();
