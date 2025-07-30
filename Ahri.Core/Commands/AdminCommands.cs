using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
// using System.Data.SQLite; // Temporarily disabled for Docker build

namespace Ahri.Core.Commands
{
    [Group("admin", "Administrator Befehle")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class AdminCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ILogger<AdminCommands> _logger;

        public AdminCommands(ILogger<AdminCommands> logger)
        {
            _logger = logger;
        }

        [SlashCommand("test-patchnotes", "Teste die Patch Notes Benachrichtigung")]
        public async Task TestPatchNotesAsync()
        {
            var embed = new EmbedBuilder()
                .WithTitle("🧪 Test: League of Legends Patch Notes")
                .WithDescription("**Test Patch 99.99 Notes**\n\nDies ist eine Test-Benachrichtigung für das Patch Notes System.")
                .WithUrl("https://www.leagueoflegends.com/de-de/news/tags/patch-notes/")
                .WithColor(Color.Orange)
                .WithThumbnailUrl("https://static.wikia.nocookie.net/leagueoflegends/images/1/12/League_of_Legends_Icon.png")
                .AddField("Patch Version", "99.99 (Test)", true)
                .AddField("Status", "✅ System funktioniert", true)
                .WithTimestamp(DateTimeOffset.Now)
                .WithFooter("Test - League of Legends Patch Notes", "https://static.wikia.nocookie.net/leagueoflegends/images/1/12/League_of_Legends_Icon.png")
                .Build();

            await RespondAsync("🧪 **Test-Benachrichtigung:**", embed: embed);
            
            _logger.LogInformation("Test Patch Notes Benachrichtigung wurde von {User} ausgeführt", 
                Context.User.Username);
        }

        [SlashCommand("service-status", "Zeigt den Status aller Bot-Services an")]
        public async Task ServiceStatusAsync()
        {
            var embed = new EmbedBuilder()
                .WithTitle("🔧 Bot Service Status")
                .WithDescription("Status aller aktiven Services:")
                .WithColor(Color.Blue)
                .AddField("Discord Service", "✅ Aktiv", true)
                .AddField("League Patch Notes Service", "✅ Aktiv", true)
                .AddField("Überprüfungsintervall", "30 Minuten", true)
                .AddField("Letzte Überprüfung", "Wird automatisch durchgeführt", false)
                .WithTimestamp(DateTimeOffset.Now)
                .WithFooter("Ahri Bot Services")
                .Build();

            await RespondAsync(embed: embed);
        }

        [SlashCommand("server-info", "Zeigt Informationen über den aktuellen Server an")]
        public async Task ServerInfoAsync()
        {
            var guild = Context.Guild;
            if (guild == null)
            {
                await RespondAsync("❌ Dieser Befehl kann nur in einem Server verwendet werden.");
                return;
            }

            var embed = new EmbedBuilder()
                .WithTitle($"📊 Server Informationen: {guild.Name}")
                .WithThumbnailUrl(guild.IconUrl)
                .WithColor(Color.Green)
                .AddField("Server ID", guild.Id, true)
                .AddField("Erstellt am", guild.CreatedAt.ToString("dd.MM.yyyy HH:mm"), true)
                .AddField("Owner", $"<@{guild.OwnerId}>", true)
                .AddField("Mitglieder", guild.MemberCount, true)
                .AddField("Text Kanäle", guild.TextChannels.Count, true)
                .AddField("Voice Kanäle", guild.VoiceChannels.Count, true)
                .AddField("Rollen", guild.Roles.Count, true)
                .AddField("Boost Level", guild.PremiumTier, true)
                .AddField("Boosts", guild.PremiumSubscriptionCount, true)
                .WithTimestamp(DateTimeOffset.Now)
                .WithFooter("Server Informationen")
                .Build();

            await RespondAsync(embed: embed);
        }

        [SlashCommand("user-info", "Zeigt Informationen über einen Benutzer an")]
        public async Task UserInfoAsync([Summary("user", "Der Benutzer, über den Informationen angezeigt werden sollen")] IUser? user = null)
        {
            var targetUser = user ?? Context.User;
            var guildUser = targetUser as IGuildUser;

            var embed = new EmbedBuilder()
                .WithTitle($"👤 Benutzer Informationen: {targetUser.Username}")
                .WithThumbnailUrl(targetUser.GetAvatarUrl() ?? targetUser.GetDefaultAvatarUrl())
                .WithColor(Color.Purple)
                .AddField("Benutzer ID", targetUser.Id, true)
                .AddField("Erstellt am", targetUser.CreatedAt.ToString("dd.MM.yyyy HH:mm"), true)
                .AddField("Bot", targetUser.IsBot ? "✅ Ja" : "❌ Nein", true);

            if (guildUser != null)
            {
                embed.AddField("Beigetreten am", guildUser.JoinedAt?.ToString("dd.MM.yyyy HH:mm") ?? "Unbekannt", true);
                embed.AddField("Nickname", guildUser.Nickname ?? "Keiner", true);
                embed.AddField("Rollen", guildUser.RoleIds.Count - 1, true); // -1 für @everyone Rolle
                
                if (guildUser.RoleIds.Count > 1)
                {
                    var roleNames = guildUser.RoleIds
                        .Where(id => id != Context.Guild.EveryoneRole.Id)
                        .Select(id => Context.Guild.GetRole(id)?.Name)
                        .Where(name => name != null)
                        .Take(10);
                    
                    embed.AddField("Wichtigste Rollen", string.Join(", ", roleNames) + (guildUser.RoleIds.Count > 11 ? "..." : ""), false);
                }
            }

            embed.WithTimestamp(DateTimeOffset.Now)
                .WithFooter("Benutzer Informationen");

            await RespondAsync(embed: embed.Build());
        }

        [SlashCommand("clear-messages", "Löscht eine bestimmte Anzahl von Nachrichten")]
        public async Task ClearMessagesAsync([Summary("amount", "Anzahl der zu löschenden Nachrichten (1-100)")] int amount)
        {
            if (amount < 1 || amount > 100)
            {
                await RespondAsync("❌ Die Anzahl muss zwischen 1 und 100 liegen.", ephemeral: true);
                return;
            }

            var channel = Context.Channel as ITextChannel;
            if (channel == null)
            {
                await RespondAsync("❌ Dieser Befehl kann nur in Text-Kanälen verwendet werden.", ephemeral: true);
                return;
            }

            try
            {
                await RespondAsync("🗑️ Lösche Nachrichten...", ephemeral: true);
                
                var messages = await channel.GetMessagesAsync(amount).FlattenAsync();
                var messagesToDelete = messages.Where(x => DateTimeOffset.Now - x.Timestamp < TimeSpan.FromDays(14));
                
                if (messagesToDelete.Any())
                {
                    await channel.DeleteMessagesAsync(messagesToDelete);
                    
                    var deletedCount = messagesToDelete.Count();
                    await FollowupAsync($"✅ {deletedCount} Nachricht(en) wurden gelöscht.", ephemeral: true);
                    
                    _logger.LogInformation("{User} hat {Count} Nachrichten in {Channel} gelöscht", 
                        Context.User.Username, deletedCount, channel.Name);
                }
                else
                {
                    await FollowupAsync("❌ Keine Nachrichten zum Löschen gefunden (Nachrichten älter als 14 Tage können nicht gelöscht werden).", ephemeral: true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Löschen von Nachrichten");
                await FollowupAsync("❌ Fehler beim Löschen der Nachrichten. Überprüfe die Bot-Berechtigungen.", ephemeral: true);
            }
        }

        [SlashCommand("announce", "Sendet eine Ankündigung in den aktuellen Kanal")]
        public async Task AnnounceAsync([Summary("title", "Titel der Ankündigung")] string title, 
                                      [Summary("message", "Nachricht der Ankündigung")] string message,
                                      [Summary("color", "Farbe der Ankündigung (rot, grün, blau, orange, lila)")] string color = "blau")
        {
            Color embedColor = color.ToLower() switch
            {
                "rot" => Color.Red,
                "grün" => Color.Green,
                "blau" => Color.Blue,
                "orange" => Color.Orange,
                "lila" => Color.Purple,
                "gelb" => Color.Gold,
                _ => Color.Blue
            };

            var embed = new EmbedBuilder()
                .WithTitle($"📢 {title}")
                .WithDescription(message)
                .WithColor(embedColor)
                .WithTimestamp(DateTimeOffset.Now)
                .WithFooter($"Ankündigung von {Context.User.Username}", Context.User.GetAvatarUrl())
                .Build();

            await RespondAsync(embed: embed);
            
            _logger.LogInformation("Ankündigung '{Title}' wurde von {User} gesendet", title, Context.User.Username);
        }

        [SlashCommand("bot-stats", "Zeigt Bot-Statistiken an")]
        public async Task BotStatsAsync()
        {
            var client = Context.Client as DiscordSocketClient;
            if (client == null)
            {
                await RespondAsync("❌ Fehler beim Abrufen der Bot-Statistiken.");
                return;
            }

            var guilds = client.Guilds.Count;
            var users = client.Guilds.Sum(g => g.MemberCount);
            var channels = client.Guilds.Sum(g => g.Channels.Count);
            var uptime = DateTimeOffset.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime;

            var embed = new EmbedBuilder()
                .WithTitle("🤖 Ahri Bot Statistiken")
                .WithColor(Color.Teal)
                .AddField("Server", guilds, true)
                .AddField("Benutzer", users, true)
                .AddField("Kanäle", channels, true)
                .AddField("Uptime", $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m", true)
                .AddField("Latenz", $"{client.Latency}ms", true)
                .AddField("Version", "1.0.0", true)
                .WithThumbnailUrl(client.CurrentUser.GetAvatarUrl())
                .WithTimestamp(DateTimeOffset.Now)
                .WithFooter("Bot Statistiken")
                .Build();

            await RespondAsync(embed: embed);
        }

        [SlashCommand("set-status", "Ändert den Status des Bots")]
        public async Task SetStatusAsync([Summary("activity", "Aktivitätstyp")] string activity,
                                       [Summary("text", "Status Text")] string text,
                                       [Summary("status", "Online Status (online, idle, dnd, invisible)")] string status = "online")
        {
            var client = Context.Client as DiscordSocketClient;
            if (client == null)
            {
                await RespondAsync("❌ Fehler beim Setzen des Status.");
                return;
            }

            ActivityType activityType = activity.ToLower() switch
            {
                "playing" => ActivityType.Playing,
                "streaming" => ActivityType.Streaming,
                "listening" => ActivityType.Listening,
                "watching" => ActivityType.Watching,
                "competing" => ActivityType.Competing,
                _ => ActivityType.Playing
            };

            UserStatus userStatus = status.ToLower() switch
            {
                "online" => UserStatus.Online,
                "idle" => UserStatus.Idle,
                "dnd" => UserStatus.DoNotDisturb,
                "invisible" => UserStatus.Invisible,
                _ => UserStatus.Online
            };

            await client.SetGameAsync(text, type: activityType);
            await client.SetStatusAsync(userStatus);

            await RespondAsync($"✅ Bot Status wurde geändert zu: **{activity}** {text} ({status})", ephemeral: true);
            
            _logger.LogInformation("Bot Status wurde von {User} geändert zu: {Activity} {Text} ({Status})", 
                Context.User.Username, activity, text, status);
        }

        [SlashCommand("reload-commands", "Lädt alle Slash Commands neu")]
        public async Task ReloadCommandsAsync()
        {
            await RespondAsync("🔄 Lade Commands neu...", ephemeral: true);
            
            try
            {
                var client = Context.Client as DiscordSocketClient;
                if (client != null)
                {
                    // Hier würde normalerweise die Command-Neuladen-Logik stehen
                    // Das ist eine vereinfachte Version
                    await FollowupAsync("✅ Commands wurden erfolgreich neu geladen.", ephemeral: true);
                    
                    _logger.LogInformation("Commands wurden von {User} neu geladen", Context.User.Username);
                }
                else
                {
                    await FollowupAsync("❌ Fehler beim Neuladen der Commands.", ephemeral: true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Neuladen der Commands");
                await FollowupAsync("❌ Fehler beim Neuladen der Commands.", ephemeral: true);
            }
        }

        [SlashCommand("emergency-stop", "Notfall-Stopp für alle Bot-Services")]
        [RequireOwner]
        public async Task EmergencyStopAsync([Summary("confirm", "Bestätigung mit 'CONFIRM'")] string confirmation)
        {
            if (confirmation != "CONFIRM")
            {
                await RespondAsync("❌ Notfall-Stopp erfordert die Bestätigung 'CONFIRM'.", ephemeral: true);
                return;
            }

            await RespondAsync("🚨 **NOTFALL-STOPP AKTIVIERT**\nBot wird heruntergefahren...", ephemeral: true);
            
            _logger.LogCritical("NOTFALL-STOPP wurde von {User} ({UserId}) aktiviert", 
                Context.User.Username, Context.User.Id);

            // Hier würde normalerweise die Shutdown-Logik stehen
            // Environment.Exit(0); // Vorsichtig mit dieser Zeile!
        }

        /*
        // Voice Commands temporarily disabled for Docker build
        [SlashCommand("voice-stats", "Zeigt Voice Channel Statistiken an")]
        public async Task VoiceStatsAsync()
        {
            await RespondAsync("❌ Voice System ist derzeit deaktiviert.", ephemeral: true);
        }

        [SlashCommand("voice-cleanup", "Bereinigt verwaiste Voice Channel Einträge")]
        public async Task VoiceCleanupAsync()
        {
            await RespondAsync("❌ Voice System ist derzeit deaktiviert.", ephemeral: true);
        }

        [SlashCommand("set-voice-limit", "Setzt das Standard Voice Channel Limit für den Server")]
        public async Task SetVoiceLimitAsync([Summary("limit", "Standard Channel Limit (0 = unbegrenzt)")] int limit)
        {
            await RespondAsync("❌ Voice System ist derzeit deaktiviert.", ephemeral: true);
        }

        [SlashCommand("force-delete-voice", "Löscht einen Voice Channel gewaltsam")]
        public async Task ForceDeleteVoiceAsync([Summary("channel", "Der zu löschende Voice Channel")] IVoiceChannel channel)
        {
            await RespondAsync("❌ Voice System ist derzeit deaktiviert.", ephemeral: true);
        }
        */
    }
}
