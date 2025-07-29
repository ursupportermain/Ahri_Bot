# Akali Bot 🎮

Ein Discord Bot für League of Legends Communities, entwickelt mit .NET 9 und Discord.Net.

![.NET Version](https://img.shields.io/badge/.NET-9.0-blue)
![Discord.Net](https://img.shields.io/badge/Discord.Net-3.18.0-7289da)
![License](https://img.shields.io/badge/license-MIT-green)

## 🌟 Features

- **⚡ Slash Commands** - Moderne Discord-Integration
- **📰 League Patch Notes** - Automatische Benachrichtigungen bei neuen Patches
- **🛠️ Admin Tools** - Verwaltungsbefehle für Server-Administratoren
- **🔒 Sichere Konfiguration** - User Secrets & Environment Variables
- **🐳 Docker Support** - Einfache Deployment-Optionen
- **📊 Logging & Monitoring** - Ausführliche Protokollierung

## 🚀 Quick Start

### Entwicklung
```bash
# Repository klonen
git clone https://github.com/ursupportermain/Akali_Bot.git
cd Akali_Bot/Akali.Core

# Bot Token konfigurieren
dotnet user-secrets set "Discord:Token" "YOUR_BOT_TOKEN"
dotnet user-secrets set "Discord:Guild:Id" "YOUR_GUILD_ID"

# League Patch Notes Channel (optional)
dotnet user-secrets set "LeaguePatchNotes:ChannelId" "YOUR_CHANNEL_ID"

# Bot starten
dotnet run
```

### Production (Docker)
```bash
# Environment Variables setzen
export DISCORD_TOKEN="your_bot_token"
export DISCORD_GUILD_ID="your_guild_id"
export LEAGUEPATCHNOTES_CHANNELID="your_channel_id"

# Mit Docker Compose starten
docker-compose up -d
```

## 🎮 Commands

### Allgemeine Commands
- `/ping` - Bot-Status überprüfen

### League of Legends
- `/patchnotes status` - Patch Notes Konfiguration anzeigen
- `/patchnotes setchannel` - Channel für Patch Notes setzen
- `/patchnotes check` - Manuell auf neue Patches prüfen

### Admin Commands (Nur für Administratoren)
- `/admin test-patchnotes` - Test-Benachrichtigung senden
- `/admin service-status` - Service-Status anzeigen

## � Dokumentation

Alle detaillierten Anleitungen finden Sie im [`docs/`](docs/) Ordner:

- **[Setup Guide](docs/SETUP_GUIDE.md)** - Schritt-für-Schritt Einrichtung
- **[League Patch Notes](docs/LEAGUE_PATCH_NOTES.md)** - Patch Notes System
- **[Docker Deployment](docs/DOCKER.md)** - Docker-spezifische Anweisungen
- **[Deployment Guide](docs/DEPLOYMENT.md)** - Vollständige Deployment-Anleitung

## 🏗️ Projektstruktur

```
Akali_Bot/
├── 📁 Akali.Core/           # Hauptprojekt
│   ├── 📁 Commands/         # Slash Command Implementierungen
│   ├── 📁 Services/         # Background Services
│   └── 📄 Program.cs        # Application Entry Point
├── 📁 docs/                 # Dokumentation
├── 📁 .github/workflows/    # CI/CD Pipelines
├── 🐳 Dockerfile           # Container Configuration
├── 🐳 docker-compose.yml   # Production Compose
└── 📄 README.md            # Diese Datei
```

## 🔧 Entwicklung

### Prerequisites
- .NET 9 SDK
- Discord Bot Token ([Discord Developer Portal](https://discord.com/developers/applications))

### Lokale Entwicklung
```bash
# Dependencies installieren
dotnet restore

# Projekt bauen
dotnet build

# Bot starten (Development Mode)
dotnet run
```

### Environment Variables
```bash
# Erforderlich
DISCORD_TOKEN=your_bot_token
DISCORD_GUILD_ID=your_guild_id

# Optional (League Patch Notes)
LEAGUEPATCHNOTES_CHANNELID=your_channel_id
LEAGUEPATCHNOTES_ROLEID=your_role_id
```

## 📜 Discord Bot Permissions

### Bot Permissions
- ✅ Send Messages
- ✅ Use Slash Commands  
- ✅ Read Message History
- ✅ View Channels
- ✅ Embed Links

### Gateway Intents
- ✅ Guilds
- ✅ Guild Messages
- ✅ Message Content
- ✅ Guild Members

## 🤝 Contributing

1. Fork das Repository
2. Erstelle einen Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Committe deine Änderungen (`git commit -m 'Add some AmazingFeature'`)
4. Push zum Branch (`git push origin feature/AmazingFeature`)
5. Öffne einen Pull Request

## 📄 License

Dieses Projekt steht unter der MIT License - siehe die [LICENSE](LICENSE) Datei für Details.

## 🔗 Links

- [Discord.Net Documentation](https://docs.discordnet.dev/)
- [.NET 9 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [League of Legends Patch Notes](https://www.leagueoflegends.com/de-de/news/tags/patch-notes/)

---

**Entwickelt mit ❤️ für die League of Legends Community**
