using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace Akali.Core.Commands
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
                .WithFooter("Akali Bot Services")
                .Build();

            await RespondAsync(embed: embed);
        }
    }
}
