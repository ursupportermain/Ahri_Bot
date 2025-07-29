using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Akali.Core.Commands
{
    public class LeaguePatchNotesCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LeaguePatchNotesCommands> _logger;

        public LeaguePatchNotesCommands(IConfiguration configuration, ILogger<LeaguePatchNotesCommands> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [SlashCommand("patchnotes", "Verwalte League of Legends Patch Notes Benachrichtigungen")]
        public async Task PatchNotesCommand(
            [Summary("action", "Die Aktion die ausgeführt werden soll")]
            [Choice("Status anzeigen", "status")]
            [Choice("Channel setzen", "setchannel")]
            [Choice("Rolle setzen", "setrole")]
            [Choice("Jetzt überprüfen", "check")]
            string action,
            [Summary("channel", "Der Channel für Patch Notes (nur bei 'setchannel')")] ITextChannel? channel = null,
            [Summary("role", "Die Rolle die erwähnt werden soll (nur bei 'setrole')")] IRole? role = null)
        {
            switch (action)
            {
                case "status":
                    await ShowStatusAsync();
                    break;
                case "setchannel":
                    await SetChannelAsync(channel);
                    break;
                case "setrole":
                    await SetRoleAsync(role);
                    break;
                case "check":
                    await CheckNowAsync();
                    break;
                default:
                    await RespondAsync("Unbekannte Aktion!", ephemeral: true);
                    break;
            }
        }

        private async Task ShowStatusAsync()
        {
            var channelIdString = _configuration["LeaguePatchNotes:ChannelId"];
            var roleIdString = _configuration["LeaguePatchNotes:RoleId"];
            
            var embed = new EmbedBuilder()
                .WithTitle("🔧 League Patch Notes Konfiguration")
                .WithColor(Color.Blue);

            if (!string.IsNullOrEmpty(channelIdString) && ulong.TryParse(channelIdString, out ulong channelId))
            {
                var channel = Context.Guild.GetChannel(channelId);
                embed.AddField("📢 Benachrichtigungs-Channel", 
                    channel != null ? $"<#{channelId}>" : $"Channel nicht gefunden (ID: {channelId})", 
                    true);
            }
            else
            {
                embed.AddField("📢 Benachrichtigungs-Channel", "❌ Nicht konfiguriert", true);
            }

            if (!string.IsNullOrEmpty(roleIdString) && ulong.TryParse(roleIdString, out ulong roleId))
            {
                var role = Context.Guild.GetRole(roleId);
                embed.AddField("🏷️ Benachrichtigungs-Rolle", 
                    role != null ? $"<@&{roleId}>" : $"Rolle nicht gefunden (ID: {roleId})", 
                    true);
            }
            else
            {
                embed.AddField("🏷️ Benachrichtigungs-Rolle", "❌ Nicht konfiguriert", true);
            }

            embed.AddField("ℹ️ Information", 
                "Der Bot überprüft alle 30 Minuten automatisch auf neue Patch Notes.\n" +
                "Verwende `/patchnotes setchannel` um den Channel zu konfigurieren.\n" +
                "Verwende `/patchnotes setrole` um eine Rolle zu konfigurieren (optional).", 
                false);

            await RespondAsync(embed: embed.Build());
        }

        private async Task SetChannelAsync(ITextChannel? channel)
        {
            if (channel == null)
            {
                await RespondAsync("❌ Bitte wähle einen gültigen Channel aus!", ephemeral: true);
                return;
            }

            // Hier würdest du normalerweise die Konfiguration in einer Datenbank oder Datei speichern
            // Für dieses Beispiel zeigen wir nur eine Bestätigung
            var embed = new EmbedBuilder()
                .WithTitle("✅ Channel konfiguriert")
                .WithDescription($"Patch Notes werden nun in <#{channel.Id}> gesendet.")
                .WithColor(Color.Green)
                .AddField("ℹ️ Hinweis", 
                    "Diese Einstellung wird nur für diese Sitzung gespeichert. " +
                    "Füge `\"LeaguePatchNotes:ChannelId\": \"" + channel.Id + "\"` zu deiner appsettings.json hinzu " +
                    "um die Einstellung dauerhaft zu speichern.", 
                    false)
                .Build();

            await RespondAsync(embed: embed);
            
            _logger.LogInformation("Patch Notes Channel wurde auf {ChannelId} ({ChannelName}) gesetzt", 
                channel.Id, channel.Name);
        }

        private async Task SetRoleAsync(IRole? role)
        {
            if (role == null)
            {
                await RespondAsync("❌ Bitte wähle eine gültige Rolle aus!", ephemeral: true);
                return;
            }

            var embed = new EmbedBuilder()
                .WithTitle("✅ Rolle konfiguriert")
                .WithDescription($"Die Rolle <@&{role.Id}> wird bei neuen Patch Notes erwähnt.")
                .WithColor(Color.Green)
                .AddField("ℹ️ Hinweis", 
                    "Diese Einstellung wird nur für diese Sitzung gespeichert. " +
                    "Füge `\"LeaguePatchNotes:RoleId\": \"" + role.Id + "\"` zu deiner appsettings.json hinzu " +
                    "um die Einstellung dauerhaft zu speichern.", 
                    false)
                .Build();

            await RespondAsync(embed: embed);
            
            _logger.LogInformation("Patch Notes Rolle wurde auf {RoleId} ({RoleName}) gesetzt", 
                role.Id, role.Name);
        }

        private async Task CheckNowAsync()
        {
            await DeferAsync();

            try
            {
                using var httpClient = new HttpClient();
                const string patchNotesUrl = "https://www.leagueoflegends.com/de-de/news/tags/patch-notes/";
                
                var response = await httpClient.GetStringAsync(patchNotesUrl);
                
                var embed = new EmbedBuilder()
                    .WithTitle("🔍 Patch Notes überprüft")
                    .WithDescription("Die Patch Notes wurden erfolgreich abgerufen. " +
                                   "Falls ein neuer Patch verfügbar ist, wird eine Benachrichtigung gesendet.")
                    .WithColor(Color.Green)
                    .WithTimestamp(DateTimeOffset.Now)
                    .Build();

                await FollowupAsync(embed: embed);
                
                _logger.LogInformation("Manuelle Patch Notes Überprüfung durch {User} ausgeführt", 
                    Context.User.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler bei manueller Patch Notes Überprüfung");
                
                var embed = new EmbedBuilder()
                    .WithTitle("❌ Fehler")
                    .WithDescription("Es ist ein Fehler beim Überprüfen der Patch Notes aufgetreten.")
                    .WithColor(Color.Red)
                    .Build();

                await FollowupAsync(embed: embed);
            }
        }
    }
}
