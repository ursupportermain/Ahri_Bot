using Discord;
using Discord.WebSocket;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Ahri.Core.Services
{
    public class LeaguePatchNotesService : BackgroundService
    {
        private readonly ILogger<LeaguePatchNotesService> _logger;
        private readonly IConfiguration _configuration;
        private readonly DiscordSocketClient _discordClient;
        private readonly HttpClient _httpClient;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30);
        private string? _lastKnownPatch;
        private readonly string _stateFilePath;

        public LeaguePatchNotesService(
            ILogger<LeaguePatchNotesService> logger,
            IConfiguration configuration,
            DiscordSocketClient discordClient,
            HttpClient httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _discordClient = discordClient;
            _httpClient = httpClient;
            _stateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "last_patch.json");
            
            // Lade den letzten bekannten Patch
            LoadLastKnownPatch();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("League Patch Notes Service gestartet");

            // Warte bis Discord bereit ist
            while (!_discordClient.ConnectionState.HasFlag(ConnectionState.Connected) && !stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckForNewPatchNotes();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fehler beim Überprüfen der Patch Notes");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckForNewPatchNotes()
        {
            try
            {
                const string patchNotesUrl = "https://www.leagueoflegends.com/de-de/news/tags/patch-notes/";
                
                var response = await _httpClient.GetStringAsync(patchNotesUrl);
                var doc = new HtmlDocument();
                doc.LoadHtml(response);

                // Suche nach dem neuesten Patch-Artikel
                var patchArticles = doc.DocumentNode
                    .SelectNodes("//article[contains(@class, 'default-article')]")
                    ?.Take(1);

                if (patchArticles == null || !patchArticles.Any())
                {
                    _logger.LogWarning("Keine Patch-Artikel gefunden");
                    return;
                }

                var latestArticle = patchArticles.First();
                var titleNode = latestArticle.SelectSingleNode(".//h2/a") ?? latestArticle.SelectSingleNode(".//h3/a");
                var linkNode = latestArticle.SelectSingleNode(".//h2/a") ?? latestArticle.SelectSingleNode(".//h3/a");

                if (titleNode == null || linkNode == null)
                {
                    _logger.LogWarning("Titel oder Link des Patch-Artikels nicht gefunden");
                    return;
                }

                var title = titleNode.InnerText.Trim();
                var link = linkNode.GetAttributeValue("href", "");
                
                if (!link.StartsWith("http"))
                {
                    link = "https://www.leagueoflegends.com" + link;
                }

                // Extrahiere Patch-Version aus dem Titel
                var patchVersion = ExtractPatchVersion(title);
                
                if (string.IsNullOrEmpty(patchVersion))
                {
                    _logger.LogWarning("Patch-Version konnte nicht aus dem Titel extrahiert werden: {Title}", title);
                    return;
                }

                // Überprüfe, ob es ein neuer Patch ist
                if (_lastKnownPatch != patchVersion)
                {
                    _logger.LogInformation("Neuer Patch gefunden: {PatchVersion}", patchVersion);
                    await SendPatchNotification(title, link, patchVersion);
                    
                    _lastKnownPatch = patchVersion;
                    SaveLastKnownPatch();
                }
                else
                {
                    _logger.LogDebug("Kein neuer Patch verfügbar. Aktueller Patch: {PatchVersion}", patchVersion);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Abrufen der Patch Notes");
            }
        }

        private string? ExtractPatchVersion(string title)
        {
            // Regex für Patch-Versionen wie "14.15", "13.24", etc.
            var regex = new Regex(@"\b(\d+)\.(\d+)\b");
            var match = regex.Match(title);
            
            return match.Success ? match.Value : null;
        }

        private async Task SendPatchNotification(string title, string link, string patchVersion)
        {
            var channelIdString = _configuration["LeaguePatchNotes:ChannelId"];
            
            if (string.IsNullOrEmpty(channelIdString) || !ulong.TryParse(channelIdString, out ulong channelId))
            {
                _logger.LogWarning("Patch Notes Channel ID ist nicht konfiguriert oder ungültig");
                return;
            }

            var channel = _discordClient.GetChannel(channelId) as ITextChannel;
            
            if (channel == null)
            {
                _logger.LogWarning("Patch Notes Channel mit ID {ChannelId} nicht gefunden", channelId);
                return;
            }

            var embed = new EmbedBuilder()
                .WithTitle($"🆕 Neue League of Legends Patch Notes!")
                .WithDescription($"**{title}**")
                .WithUrl(link)
                .WithColor(Color.Gold)
                .WithThumbnailUrl("https://static.wikia.nocookie.net/leagueoflegends/images/1/12/League_of_Legends_Icon.png")
                .AddField("Patch Version", patchVersion, true)
                .AddField("Link", $"[Patch Notes lesen]({link})", true)
                .WithTimestamp(DateTimeOffset.Now)
                .WithFooter("League of Legends Patch Notes", "https://static.wikia.nocookie.net/leagueoflegends/images/1/12/League_of_Legends_Icon.png")
                .Build();

            try
            {
                var roleIdString = _configuration["LeaguePatchNotes:RoleId"];
                var message = "";
                
                if (!string.IsNullOrEmpty(roleIdString) && ulong.TryParse(roleIdString, out ulong roleId))
                {
                    message = $"<@&{roleId}> ";
                }

                await channel.SendMessageAsync(text: message, embed: embed);
                _logger.LogInformation("Patch Notes Benachrichtigung gesendet für Patch {PatchVersion}", patchVersion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Senden der Patch Notes Benachrichtigung");
            }
        }

        private void LoadLastKnownPatch()
        {
            try
            {
                if (File.Exists(_stateFilePath))
                {
                    var json = File.ReadAllText(_stateFilePath);
                    var state = JsonSerializer.Deserialize<PatchState>(json);
                    _lastKnownPatch = state?.LastPatchVersion;
                    _logger.LogInformation("Letzter bekannter Patch geladen: {PatchVersion}", _lastKnownPatch ?? "Unbekannt");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Fehler beim Laden des letzten Patch-Status");
            }
        }

        private void SaveLastKnownPatch()
        {
            try
            {
                var state = new PatchState { LastPatchVersion = _lastKnownPatch };
                var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_stateFilePath, json);
                _logger.LogDebug("Patch-Status gespeichert: {PatchVersion}", _lastKnownPatch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Speichern des Patch-Status");
            }
        }

        private class PatchState
        {
            public string? LastPatchVersion { get; set; }
        }
    }
}
