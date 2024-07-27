using service;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose() 
    .WriteTo.Console() 
    .WriteTo.File("logs/service-errors.log", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error) 
    .WriteTo.File("logs/service-info.log", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
    .CreateLogger();

builder.Services.AddSerilog(); 

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddSingleton<IDatabaseHandler, DatabaseHandler>();
builder.Services.AddSingleton<INatsStatsTracker, NatsStatsTracker>();
builder.Services.AddHostedService<MarketOrderListener>();
builder.Services.AddHostedService<MarketHistoryListener>();
builder.Services.AddHostedService<StatsWorker>();

var host = builder.Build();
host.Run();
