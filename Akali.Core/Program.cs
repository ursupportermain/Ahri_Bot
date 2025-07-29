using Akali.Core.Services;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Add configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables();

// Add services
builder.Services.AddSingleton<DiscordSocketClient>(provider =>
{
    var config = new DiscordSocketConfig()
    {
        GatewayIntents = GatewayIntents.Guilds | 
                        GatewayIntents.GuildMessages | 
                        GatewayIntents.MessageContent |
                        GatewayIntents.GuildMembers,
        LogLevel = LogSeverity.Info,
        MessageCacheSize = 100
    };
    return new DiscordSocketClient(config);
});

builder.Services.AddHostedService<DiscordService>();

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

var app = builder.Build();

Console.WriteLine("Starting Akali Bot...");

try
{
    await app.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Application terminated unexpectedly: {ex.Message}");
    throw;
}
