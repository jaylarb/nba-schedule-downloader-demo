using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nba.Business;
using Nba.Business.Interface;
using Nba.Data;
using NLog.Extensions.Logging;

Console.WriteLine("Initializing program...");

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddNLog();

// Configure appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Register services
builder.Services.AddSingleton<AppSettings>();
builder.Services.AddTransient<HttpClient>();
builder.Services.AddTransient<IHttpClientWrapper, HttpClientWrapper>();
builder.Services.AddSingleton<ISeasonScheduleManager, SeasonScheduleManager>();
builder.Services.AddSingleton<ISportRadarService, SportRadarService>();

// Register configuration
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Register data services
string connectionString = builder.Configuration.GetConnectionString("Nba")
    ?? throw new NotImplementedException("You must configure Nba connection string");
builder.Services.AddDataServices(connectionString);

using IHost host = builder.Build();

ILogger logger = host.Services.GetRequiredService<ILogger<Program>>();

Console.WriteLine("Executing program...");
logger.LogTrace("Executing program...");

// Run the program
ISeasonScheduleManager manager = host.Services.GetRequiredService<ISeasonScheduleManager>();
bool success = await manager.GetSeasonSchedule();

logger.LogTrace($"Program finished! Successful?: {success}");
Console.WriteLine($"Program finished! Successful?: {(success ? success : " False. See log for details.")}");