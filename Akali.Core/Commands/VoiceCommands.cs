using System.Data.SQLite;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Akali.Core.Commands
{
    [Group("voice", "Voice Channel Management")]
    public class VoiceCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ILogger<VoiceCommands> _logger;
        private const string DatabasePath = "voice.db";

        public VoiceCommands(ILogger<VoiceCommands> logger)
        {
            _logger = logger;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SQLiteConnection($"Data Source={DatabasePath}");
            connection.Open();

            string createGuildTable = @"
                CREATE TABLE IF NOT EXISTS guild (
                    guildID INTEGER PRIMARY KEY,
                    ownerID INTEGER,
                    voiceChannelID INTEGER,
                    voiceCategoryID INTEGER
                )";

            string createVoiceChannelTable = @"
                CREATE TABLE IF NOT EXISTS voiceChannel (
                    userID INTEGER PRIMARY KEY,
                    voiceID INTEGER
                )";

            string createUserSettingsTable = @"
                CREATE TABLE IF NOT EXISTS userSettings (
                    userID INTEGER PRIMARY KEY,
                    channelName TEXT,
                    channelLimit INTEGER
                )";

            string createGuildSettingsTable = @"
                CREATE TABLE IF NOT EXISTS guildSettings (
                    guildID INTEGER PRIMARY KEY,
                    defaultChannelName TEXT,
                    channelLimit INTEGER
                )";

            using var command = new SQLiteCommand(createGuildTable, connection);
            command.ExecuteNonQuery();

            command.CommandText = createVoiceChannelTable;
            command.ExecuteNonQuery();

            command.CommandText = createUserSettingsTable;
            command.ExecuteNonQuery();

            command.CommandText = createGuildSettingsTable;
            command.ExecuteNonQuery();
        }

        [SlashCommand("setup", "Richtet das Voice Channel System ein")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetupAsync([Summary("category-name", "Name der Kategorie für Voice Channels")] string categoryName,
                                   [Summary("channel-name", "Name des Join-to-Create Channels")] string channelName)
        {
            try
            {
                await DeferAsync(ephemeral: true);

                var guild = Context.Guild;
                var category = await guild.CreateCategoryChannelAsync(categoryName);
                var voiceChannel = await guild.CreateVoiceChannelAsync(channelName, props => props.CategoryId = category.Id);

                using var connection = new SQLiteConnection($"Data Source={DatabasePath}");
                connection.Open();

                string query = @"
                    INSERT OR REPLACE INTO guild (guildID, ownerID, voiceChannelID, voiceCategoryID) 
                    VALUES (@guildId, @ownerId, @channelId, @categoryId)";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@guildId", guild.Id);
                command.Parameters.AddWithValue("@ownerId", Context.User.Id);
                command.Parameters.AddWithValue("@channelId", voiceChannel.Id);
                command.Parameters.AddWithValue("@categoryId", category.Id);
                command.ExecuteNonQuery();

                var embed = new EmbedBuilder()
                    .WithTitle("✅ Voice System Setup Complete")
                    .WithDescription($"Voice Channel System wurde erfolgreich eingerichtet!")
                    .AddField("Kategorie", categoryName, true)
                    .AddField("Join Channel", channelName, true)
                    .WithColor(Color.Green)
                    .WithTimestamp(DateTimeOffset.Now)
                    .Build();

                await FollowupAsync(embed: embed, ephemeral: true);
                _logger.LogInformation("Voice System setup by {User} in guild {Guild}", Context.User.Username, guild.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during voice setup");
                await FollowupAsync("❌ Fehler beim Setup des Voice Systems.", ephemeral: true);
            }
        }

        [SlashCommand("lock", "Sperrt deinen Voice Channel")]
        public async Task LockAsync()
        {
            try
            {
                var channelId = GetUserVoiceChannel(Context.User.Id);
                if (channelId == null)
                {
                    await RespondAsync("❌ Du besitzt keinen Voice Channel.", ephemeral: true);
                    return;
                }

                var channel = Context.Guild.GetVoiceChannel(channelId.Value);
                if (channel == null)
                {
                    await RespondAsync("❌ Channel nicht gefunden.", ephemeral: true);
                    return;
                }

                await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, 
                    new OverwritePermissions(connect: PermValue.Deny));

                await RespondAsync("🔒 Voice Channel wurde gesperrt!", ephemeral: true);
                _logger.LogInformation("Voice channel locked by {User}", Context.User.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error locking voice channel");
                await RespondAsync("❌ Fehler beim Sperren des Channels.", ephemeral: true);
            }
        }

        [SlashCommand("unlock", "Entsperrt deinen Voice Channel")]
        public async Task UnlockAsync()
        {
            try
            {
                var channelId = GetUserVoiceChannel(Context.User.Id);
                if (channelId == null)
                {
                    await RespondAsync("❌ Du besitzt keinen Voice Channel.", ephemeral: true);
                    return;
                }

                var channel = Context.Guild.GetVoiceChannel(channelId.Value);
                if (channel == null)
                {
                    await RespondAsync("❌ Channel nicht gefunden.", ephemeral: true);
                    return;
                }

                await channel.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, 
                    new OverwritePermissions(connect: PermValue.Allow));

                await RespondAsync("🔓 Voice Channel wurde entsperrt!", ephemeral: true);
                _logger.LogInformation("Voice channel unlocked by {User}", Context.User.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlocking voice channel");
                await RespondAsync("❌ Fehler beim Entsperren des Channels.", ephemeral: true);
            }
        }

        [SlashCommand("permit", "Erlaubt einem Benutzer den Zugang zu deinem Channel")]
        public async Task PermitAsync([Summary("user", "Benutzer dem Zugang gewährt werden soll")] IUser user)
        {
            try
            {
                var channelId = GetUserVoiceChannel(Context.User.Id);
                if (channelId == null)
                {
                    await RespondAsync("❌ Du besitzt keinen Voice Channel.", ephemeral: true);
                    return;
                }

                var channel = Context.Guild.GetVoiceChannel(channelId.Value);
                if (channel == null)
                {
                    await RespondAsync("❌ Channel nicht gefunden.", ephemeral: true);
                    return;
                }

                var guildUser = Context.Guild.GetUser(user.Id);
                await channel.AddPermissionOverwriteAsync(guildUser, 
                    new OverwritePermissions(connect: PermValue.Allow));

                await RespondAsync($"✅ {user.Mention} wurde Zugang zu deinem Channel gewährt.", ephemeral: true);
                _logger.LogInformation("User {TargetUser} permitted to channel by {User}", user.Username, Context.User.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error permitting user");
                await RespondAsync("❌ Fehler beim Gewähren des Zugangs.", ephemeral: true);
            }
        }

        [SlashCommand("reject", "Entfernt einen Benutzer aus deinem Channel")]
        public async Task RejectAsync([Summary("user", "Benutzer der entfernt werden soll")] IUser user)
        {
            try
            {
                var channelId = GetUserVoiceChannel(Context.User.Id);
                if (channelId == null)
                {
                    await RespondAsync("❌ Du besitzt keinen Voice Channel.", ephemeral: true);
                    return;
                }

                var channel = Context.Guild.GetVoiceChannel(channelId.Value);
                if (channel == null)
                {
                    await RespondAsync("❌ Channel nicht gefunden.", ephemeral: true);
                    return;
                }

                var guildUser = Context.Guild.GetUser(user.Id);
                
                // Move user to main voice channel if they're in the private channel
                if (guildUser.VoiceChannel?.Id == channelId)
                {
                    var mainChannelId = GetGuildMainVoiceChannel(Context.Guild.Id);
                    if (mainChannelId.HasValue)
                    {
                        var mainChannel = Context.Guild.GetVoiceChannel(mainChannelId.Value);
                        if (mainChannel != null)
                        {
                            await guildUser.ModifyAsync(props => props.Channel = mainChannel);
                        }
                    }
                }

                await channel.AddPermissionOverwriteAsync(guildUser, 
                    new OverwritePermissions(connect: PermValue.Deny));

                await RespondAsync($"❌ {user.Mention} wurde aus deinem Channel entfernt.", ephemeral: true);
                _logger.LogInformation("User {TargetUser} rejected from channel by {User}", user.Username, Context.User.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting user");
                await RespondAsync("❌ Fehler beim Entfernen des Benutzers.", ephemeral: true);
            }
        }

        [SlashCommand("limit", "Setzt das Benutzerlimit für deinen Channel")]
        public async Task LimitAsync([Summary("limit", "Maximale Anzahl Benutzer (0 = unbegrenzt)")] int limit)
        {
            try
            {
                if (limit < 0 || limit > 99)
                {
                    await RespondAsync("❌ Limit muss zwischen 0 und 99 liegen.", ephemeral: true);
                    return;
                }

                var channelId = GetUserVoiceChannel(Context.User.Id);
                if (channelId == null)
                {
                    await RespondAsync("❌ Du besitzt keinen Voice Channel.", ephemeral: true);
                    return;
                }

                var channel = Context.Guild.GetVoiceChannel(channelId.Value);
                if (channel == null)
                {
                    await RespondAsync("❌ Channel nicht gefunden.", ephemeral: true);
                    return;
                }

                await channel.ModifyAsync(props => props.UserLimit = limit == 0 ? null : limit);

                // Save user preference
                SaveUserSetting(Context.User.Id, null, limit);

                var limitText = limit == 0 ? "unbegrenzt" : limit.ToString();
                await RespondAsync($"✅ Channel Limit wurde auf {limitText} gesetzt!", ephemeral: true);
                _logger.LogInformation("Channel limit set to {Limit} by {User}", limit, Context.User.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting channel limit");
                await RespondAsync("❌ Fehler beim Setzen des Limits.", ephemeral: true);
            }
        }

        [SlashCommand("name", "Ändert den Namen deines Channels")]
        public async Task NameAsync([Summary("name", "Neuer Channel Name")] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
                {
                    await RespondAsync("❌ Name darf nicht leer sein und maximal 100 Zeichen haben.", ephemeral: true);
                    return;
                }

                var channelId = GetUserVoiceChannel(Context.User.Id);
                if (channelId == null)
                {
                    await RespondAsync("❌ Du besitzt keinen Voice Channel.", ephemeral: true);
                    return;
                }

                var channel = Context.Guild.GetVoiceChannel(channelId.Value);
                if (channel == null)
                {
                    await RespondAsync("❌ Channel nicht gefunden.", ephemeral: true);
                    return;
                }

                await channel.ModifyAsync(props => props.Name = name);

                // Save user preference
                SaveUserSetting(Context.User.Id, name, null);

                await RespondAsync($"✅ Channel Name wurde zu **{name}** geändert!", ephemeral: true);
                _logger.LogInformation("Channel name changed to {Name} by {User}", name, Context.User.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing channel name");
                await RespondAsync("❌ Fehler beim Ändern des Channel Namens.", ephemeral: true);
            }
        }

        [SlashCommand("claim", "Übernimmt die Kontrolle über einen Channel")]
        public async Task ClaimAsync()
        {
            try
            {
                var guildUser = Context.User as SocketGuildUser;
                if (guildUser?.VoiceChannel == null)
                {
                    await RespondAsync("❌ Du musst in einem Voice Channel sein.", ephemeral: true);
                    return;
                }

                var channelId = guildUser.VoiceChannel.Id;
                var currentOwnerId = GetChannelOwner(channelId);

                if (currentOwnerId == null)
                {
                    await RespondAsync("❌ Dieser Channel kann nicht übernommen werden.", ephemeral: true);
                    return;
                }

                // Check if current owner is still in the channel
                var currentOwner = Context.Guild.GetUser(currentOwnerId.Value);
                if (currentOwner?.VoiceChannel?.Id == channelId)
                {
                    await RespondAsync($"❌ Dieser Channel gehört bereits {currentOwner.Mention} und sie sind noch im Channel!", ephemeral: true);
                    return;
                }

                // Transfer ownership
                using var connection = new SQLiteConnection($"Data Source={DatabasePath}");
                connection.Open();

                string query = "UPDATE voiceChannel SET userID = @newOwner WHERE voiceID = @channelId";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@newOwner", Context.User.Id);
                command.Parameters.AddWithValue("@channelId", channelId);
                command.ExecuteNonQuery();

                await RespondAsync("✅ Du bist jetzt der Owner dieses Channels!", ephemeral: true);
                _logger.LogInformation("Channel {ChannelId} claimed by {User}", channelId, Context.User.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error claiming channel");
                await RespondAsync("❌ Fehler beim Übernehmen des Channels.", ephemeral: true);
            }
        }

        [SlashCommand("help", "Zeigt alle Voice Commands an")]
        public async Task HelpAsync()
        {
            var embed = new EmbedBuilder()
                .WithTitle("🔊 Voice Commands Help")
                .WithDescription("Alle verfügbaren Voice Channel Befehle:")
                .WithColor(Color.Blue)
                .AddField("🔧 Setup", "`/voice setup` - Richtet das Voice System ein (Admin)", false)
                .AddField("🔒 Lock/Unlock", "`/voice lock` - Sperrt deinen Channel\n`/voice unlock` - Entsperrt deinen Channel", false)
                .AddField("👥 Benutzer Management", "`/voice permit @user` - Erlaubt Zugang\n`/voice reject @user` - Verweigert Zugang", false)
                .AddField("⚙️ Channel Settings", "`/voice name <name>` - Ändert Channel Namen\n`/voice limit <zahl>` - Setzt Benutzerlimit", false)
                .AddField("👑 Ownership", "`/voice claim` - Übernimmt Channel wenn Owner weg ist", false)
                .WithTimestamp(DateTimeOffset.Now)
                .WithFooter("Akali Bot Voice System")
                .Build();

            await RespondAsync(embed: embed, ephemeral: true);
        }

        private ulong? GetUserVoiceChannel(ulong userId)
        {
            using var connection = new SQLiteConnection($"Data Source={DatabasePath}");
            connection.Open();

            string query = "SELECT voiceID FROM voiceChannel WHERE userID = @userId";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            var result = command.ExecuteScalar();
            return result != null ? Convert.ToUInt64(result) : null;
        }

        private ulong? GetChannelOwner(ulong channelId)
        {
            using var connection = new SQLiteConnection($"Data Source={DatabasePath}");
            connection.Open();

            string query = "SELECT userID FROM voiceChannel WHERE voiceID = @channelId";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@channelId", channelId);

            var result = command.ExecuteScalar();
            return result != null ? Convert.ToUInt64(result) : null;
        }

        private ulong? GetGuildMainVoiceChannel(ulong guildId)
        {
            using var connection = new SQLiteConnection($"Data Source={DatabasePath}");
            connection.Open();

            string query = "SELECT voiceChannelID FROM guild WHERE guildID = @guildId";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@guildId", guildId);

            var result = command.ExecuteScalar();
            return result != null ? Convert.ToUInt64(result) : null;
        }

        private void SaveUserSetting(ulong userId, string? channelName, int? channelLimit)
        {
            using var connection = new SQLiteConnection($"Data Source={DatabasePath}");
            connection.Open();

            string query = @"
                INSERT OR REPLACE INTO userSettings (userID, channelName, channelLimit) 
                VALUES (
                    @userId, 
                    COALESCE(@channelName, (SELECT channelName FROM userSettings WHERE userID = @userId)), 
                    COALESCE(@channelLimit, (SELECT channelLimit FROM userSettings WHERE userID = @userId))
                )";

            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@channelName", channelName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@channelLimit", channelLimit ?? (object)DBNull.Value);
            command.ExecuteNonQuery();
        }
    }
}
