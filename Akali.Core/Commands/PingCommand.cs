using Discord;
using Discord.Interactions;

namespace discord.core.Commands
{
    public class SlashCommandHandler : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("Ping", "Pings the bot to check if it's online")]
        public async Task PingAsync()
        {
            var embed = new EmbedBuilder()
                .WithTitle("Pong!")
                .WithDescription("The bot is online and responsive.")
                .WithColor(Color.Green)
                .Build();

            await RespondAsync(embed: embed);
        }
        
    }
}