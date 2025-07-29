using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Akali.Core.Services
{
    public class DiscordService(
        IConfiguration configuration,
        DiscordSocketClient discordSocketClient,
        IServiceProvider serviceProvider,
        ILogger<DiscordService> logger)
        : BackgroundService
    {
        private readonly InteractionService _interactionService = new InteractionService(discordSocketClient, new InteractionServiceConfig());

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("DiscordService Gestartet");
            using var scope = serviceProvider.CreateScope();
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), scope.ServiceProvider);

            discordSocketClient.Log += LogAsync;
            discordSocketClient.Ready += ReadyAsync;
            discordSocketClient.SlashCommandExecuted += SlashCommandExecutedAsync;

            try
            {
                var token = configuration["Discord:Token"];
                    Console.WriteLine($"TOKEN ZUM TEST: {(string.IsNullOrEmpty(token) ? "NICHT GELADEN" : "GEFUNDEN")}");
                if (string.IsNullOrEmpty(token))
                {
                    throw new InvalidOperationException("Bot token is missing from configuration.");
                }

                await discordSocketClient.LoginAsync(TokenType.Bot, token);
                logger.LogInformation("Login Erfolgreich");
                
                await discordSocketClient.StartAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler beim Starten des Discord Clients");
                Console.WriteLine($"Fehler beim Starten des Discord Clients: {ex.Message}");
                throw;
            }

            stoppingToken.Register(async () =>
            {
                await discordSocketClient.StopAsync();
                logger.LogInformation("DiscordService Gestoppt");
            });
        }

        private async Task SlashCommandExecutedAsync(SocketSlashCommand command)
        {
            var socketInteractionContext = new SocketInteractionContext(discordSocketClient, command);
            try
            {
                await _interactionService.ExecuteCommandAsync(socketInteractionContext, serviceProvider);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler beim Ausführen des Slash Commands: {CommandName}", command.CommandName);
                await command.RespondAsync($"Ein Fehler ist bei der Ausführung des Befehls '{command.CommandName}' aufgetreten.", ephemeral: true);
            }
        }

        private async Task ReadyAsync()
        {
            try
            {
                await _interactionService.RegisterCommandsGloballyAsync();
                logger.LogInformation("Globale Slash-Befehle erfolgreich registriert.");

                var guildIdConfig = configuration["Discord:Guild:Id"];
                if (ulong.TryParse(guildIdConfig, out ulong guildId))
                {
                    await _interactionService.RegisterCommandsToGuildAsync(guildId);
                    logger.LogInformation("Guild Slash-Befehle erfolgreich registriert für Guild: {GuildId}", guildId);
                }
                else
                {
                    logger.LogWarning("Guild ID nicht konfiguriert oder ungültig. Guild-spezifische Befehle werden nicht registriert.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fehler beim Registrieren der Slash-Befehle.");
                Console.WriteLine($"Fehler beim Registrieren der Slash-Befehle: {ex.Message}");
            }
        }

        private Task LogAsync(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    logger.LogCritical(msg.Exception, "Discord Kritisch: {Message}", msg.Message);
                    break;
                case LogSeverity.Error:
                    logger.LogError(msg.Exception, "Discord Fehler: {Message}", msg.Message);
                    break;
                case LogSeverity.Warning:
                    logger.LogWarning("Discord Warnung: {Message}", msg.Message);
                    break;
                case LogSeverity.Info:
                    logger.LogInformation("Discord Info: {Message}", msg.Message);
                    break;
                case LogSeverity.Verbose:
                    logger.LogTrace("Discord Verbose: {Message}", msg.Message);
                    break;
                case LogSeverity.Debug:
                default:
                    logger.LogDebug("Discord Debug: {Message}", msg.Message);
                    break;
            }
            return Task.CompletedTask;
        }

    }
}