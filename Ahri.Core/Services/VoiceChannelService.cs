using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Data.SQLite;

namespace Ahri.Core.Services
{
    public class VoiceChannelService : BackgroundService
    {
        private readonly DiscordSocketClient _client;
        private readonly ILogger<VoiceChannelService> _logger;
        private const string DatabasePath = "voice.db";
        private readonly ConcurrentDictionary<ulong, DateTime> _userCooldowns = new();
        private readonly ConcurrentDictionary<ulong, CancellationTokenSource> _channelMonitors = new();

        public VoiceChannelService(DiscordSocketClient client, ILogger<VoiceChannelService> logger)
        {
            _client = client;
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("VoiceChannelService starting...");
            
            // Wait for the client to be ready
            while (_client.ConnectionState != ConnectionState.Connected)
            {
                _logger.LogInformation("Waiting for Discord client to connect... Current state: {State}", _client.ConnectionState);
                await Task.Delay(1000, stoppingToken);
                if (stoppingToken.IsCancellationRequested) return;
            }

            _client.UserVoiceStateUpdated += HandleVoiceStateUpdate;
            _logger.LogInformation("VoiceChannelService started and event handler registered");

            // Keep the service running
            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Service is being stopped
                _client.UserVoiceStateUpdated -= HandleVoiceStateUpdate;
                _logger.LogInformation("VoiceChannelService stopped");
            }
        }

        private async Task HandleVoiceStateUpdate(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            try
            {
                _logger.LogInformation("Voice state update: User {User} moved from {Before} to {After}", 
                    user.Username, before.VoiceChannel?.Name ?? "null", after.VoiceChannel?.Name ?? "null");

                if (user is not SocketGuildUser guildUser) return;

                var guild = guildUser.Guild;
                var guildId = guild.Id;
                var userId = user.Id;

                // Get guild voice configuration
                var voiceConfig = GetGuildVoiceConfig(guildId);
                if (voiceConfig == null) 
                {
                    _logger.LogInformation("No voice config found for guild {Guild}", guild.Name);
                    return;
                }

                _logger.LogInformation("Voice config found - Join Channel: {JoinChannel}, Category: {Category}", 
                    voiceConfig.VoiceChannelId, voiceConfig.VoiceCategoryId);

                // Handle joining the "Join to Create" channel
                if (after.VoiceChannel?.Id == voiceConfig.VoiceChannelId)
                {
                    _logger.LogInformation("User {User} joined the Join-to-Create channel", user.Username);
                    await HandleJoinToCreate(guildUser, voiceConfig);
                }

                // Handle leaving a temporary channel
                if (before.VoiceChannel != null && after.VoiceChannel?.Id != before.VoiceChannel.Id)
                {
                    await HandleChannelLeave(before.VoiceChannel, userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling voice state update");
            }
        }

        private async Task HandleJoinToCreate(SocketGuildUser user, GuildVoiceConfig config)
        {
            var userId = user.Id;
            var guild = user.Guild;

            // Check cooldown
            if (_userCooldowns.ContainsKey(userId))
            {
                var timeSinceLastCreate = DateTime.UtcNow - _userCooldowns[userId];
                if (timeSinceLastCreate.TotalSeconds < 15)
                {
                    try
                    {
                        await user.SendMessageAsync("⏰ Du erstellst Channels zu schnell! 15 Sekunden Cooldown.");
                        await Task.Delay(15000); // 15 second cooldown
                    }
                    catch
                    {
                        // User has DMs disabled, ignore
                    }
                }
            }

            _userCooldowns[userId] = DateTime.UtcNow;

            // Get user settings
            var userSettings = GetUserSettings(userId);
            var guildSettings = GetGuildSettings(guild.Id);

            // Determine channel name and limit
            string channelName;
            int channelLimit;

            if (userSettings != null)
            {
                channelName = userSettings.ChannelName ?? $"{user.Username}'s Channel";
                channelLimit = userSettings.ChannelLimit;
                
                if (channelLimit == 0 && guildSettings != null)
                {
                    channelLimit = guildSettings.ChannelLimit;
                }
            }
            else if (guildSettings != null)
            {
                channelName = guildSettings.DefaultChannelName?.Replace("{username}", user.Username) ?? $"{user.Username}'s Channel";
                channelLimit = guildSettings.ChannelLimit;
            }
            else
            {
                channelName = $"{user.Username}'s Channel";
                channelLimit = 0;
            }

            try
            {
                // Create the voice channel
                var category = guild.GetCategoryChannel(config.VoiceCategoryId);
                var newChannel = await guild.CreateVoiceChannelAsync(channelName, props =>
                {
                    props.CategoryId = category?.Id;
                    props.UserLimit = channelLimit == 0 ? null : channelLimit;
                });

                // Set permissions
                await newChannel.AddPermissionOverwriteAsync(_client.CurrentUser, 
                    new OverwritePermissions(connect: PermValue.Allow, viewChannel: PermValue.Allow));
                await newChannel.AddPermissionOverwriteAsync(user, 
                    new OverwritePermissions(connect: PermValue.Allow, viewChannel: PermValue.Allow, manageChannel: PermValue.Allow));

                // Move user to the new channel
                await user.ModifyAsync(props => props.Channel = newChannel);

                // Store in database
                StoreUserChannel(userId, newChannel.Id);

                // Start monitoring the channel
                var cts = new CancellationTokenSource();
                _channelMonitors[newChannel.Id] = cts;
                _ = Task.Run(async () => await MonitorChannelForEmptiness(newChannel.Id, userId, cts.Token));

                _logger.LogInformation("Created voice channel {ChannelName} for user {User}", channelName, user.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating voice channel for user {User}", user.Username);
            }
        }

        private Task HandleChannelLeave(SocketVoiceChannel channel, ulong userId)
        {
            // Check if this was a temporary channel owned by someone
            var channelOwner = GetChannelOwner(channel.Id);
            if (channelOwner == null) return Task.CompletedTask;

            // If the channel is empty, it will be deleted by the monitor
            // We don't need to do anything special here
            return Task.CompletedTask;
        }

        private async Task MonitorChannelForEmptiness(ulong channelId, ulong ownerId, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(5000, cancellationToken); // Check every 5 seconds

                    // Get the channel from Discord
                    var channel = _client.GetChannel(channelId) as SocketVoiceChannel;
                    if (channel == null)
                    {
                        // Channel was deleted externally
                        RemoveUserChannel(ownerId);
                        _channelMonitors.TryRemove(channelId, out _);
                        break;
                    }

                    if (channel.ConnectedUsers.Count == 0)
                    {
                        // Channel is empty, delete it
                        await channel.DeleteAsync();
                        RemoveUserChannel(ownerId);
                        _channelMonitors.TryRemove(channelId, out _);
                        
                        _logger.LogInformation("Deleted empty voice channel {ChannelId} owned by {UserId}", channelId, ownerId);
                        break;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // Monitor was cancelled, this is expected
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error monitoring voice channel {ChannelId}", channelId);
            }
        }

        private GuildVoiceConfig? GetGuildVoiceConfig(ulong guildId)
        {
            using var connection = new SQLiteConnection($"Data Source={DatabasePath}");
            connection.Open();

            string query = "SELECT voiceChannelID, voiceCategoryID FROM guild WHERE guildID = @guildId";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@guildId", guildId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new GuildVoiceConfig
                {
                    VoiceChannelId = Convert.ToUInt64(reader["voiceChannelID"]),
                    VoiceCategoryId = Convert.ToUInt64(reader["voiceCategoryID"])
                };
            }

            return null;
        }

        private UserSettings? GetUserSettings(ulong userId)
        {
            using var connection = new SQLiteConnection($"Data Source={DatabasePath}");
            connection.Open();

            string query = "SELECT channelName, channelLimit FROM userSettings WHERE userID = @userId";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new UserSettings
                {
                    ChannelName = reader["channelName"] as string,
                    ChannelLimit = Convert.ToInt32(reader["channelLimit"])
                };
            }

            return null;
        }

        private GuildSettings? GetGuildSettings(ulong guildId)
        {
            using var connection = new SQLiteConnection($"Data Source={DatabasePath}");
            connection.Open();

            string query = "SELECT defaultChannelName, channelLimit FROM guildSettings WHERE guildID = @guildId";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@guildId", guildId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new GuildSettings
                {
                    DefaultChannelName = reader["defaultChannelName"] as string,
                    ChannelLimit = Convert.ToInt32(reader["channelLimit"])
                };
            }

            return null;
        }

        private void StoreUserChannel(ulong userId, ulong channelId)
        {
            using var connection = new SQLiteConnection($"Data Source={DatabasePath}");
            connection.Open();

            string query = "INSERT OR REPLACE INTO voiceChannel (userID, voiceID) VALUES (@userId, @channelId)";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@channelId", channelId);
            command.ExecuteNonQuery();
        }

        private void RemoveUserChannel(ulong userId)
        {
            using var connection = new SQLiteConnection($"Data Source={DatabasePath}");
            connection.Open();

            string query = "DELETE FROM voiceChannel WHERE userID = @userId";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.ExecuteNonQuery();
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

        public override void Dispose()
        {
            // Cancel all channel monitors
            foreach (var monitor in _channelMonitors.Values)
            {
                monitor.Cancel();
                monitor.Dispose();
            }
            _channelMonitors.Clear();

            base.Dispose();
        }
    }

    public class GuildVoiceConfig
    {
        public ulong VoiceChannelId { get; set; }
        public ulong VoiceCategoryId { get; set; }
    }

    public class UserSettings
    {
        public string? ChannelName { get; set; }
        public int ChannelLimit { get; set; }
    }

    public class GuildSettings
    {
        public string? DefaultChannelName { get; set; }
        public int ChannelLimit { get; set; }
    }
}
