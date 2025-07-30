---
layout: default
title: Home
nav_order: 1
---

# Ahri Bot Documentation 🎮

Willkommen zur Dokumentation des Ahri Bots - einem Discord Bot für League of Legends Communities!

[![GitHub](https://img.shields.io/badge/GitHub-Repository-181717?logo=github)](https://github.com/ursupportermain/Ahri_Bot)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Discord.Net](https://img.shields.io/badge/Discord.Net-3.18.0-7289DA?logo=discord)](https://github.com/discord-net/Discord.Net)

## 🚀 Quick Start

```bash
# Repository klonen
git clone https://github.com/ursupportermain/Ahri_Bot.git
cd Ahri_Bot/Ahri.Core

# Bot Token konfigurieren
dotnet user-secrets set "Discord:Token" "YOUR_BOT_TOKEN"
dotnet user-secrets set "Discord:Guild:Id" "YOUR_GUILD_ID"

# Bot starten
dotnet run
```

## 📖 Dokumentation

<div class="grid">
  <div class="grid-item">
    <h3>🛠️ <a href="setup-guide">Setup Guide</a></h3>
    <p>Schritt-für-Schritt Anleitung zur Bot-Einrichtung</p>
  </div>
  
  <div class="grid-item">
    <h3>📰 <a href="league-patch-notes">League Patch Notes</a></h3>
    <p>Automatische Benachrichtigungen für neue LoL Patches</p>
  </div>
  
  <div class="grid-item">
    <h3>⚡ <a href="commands">Commands</a></h3>
    <p>Alle verfügbaren Bot-Befehle im Überblick</p>
  </div>
  
  <div class="grid-item">
    <h3>🐳 <a href="docker">Docker</a></h3>
    <p>Container-basierte Deployment-Optionen</p>
  </div>
  
  <div class="grid-item">
    <h3>🚀 <a href="deployment">Deployment</a></h3>
    <p>Vollständige Anleitung für Production Deployment</p>
  </div>
</div>

## 🌟 Features

- **⚡ Slash Commands** - Moderne Discord-Integration
- **📰 League Patch Notes** - Automatische Benachrichtigungen bei neuen Patches
- **🛠️ Admin Tools** - Verwaltungsbefehle für Server-Administratoren
- **🔒 Sichere Konfiguration** - User Secrets & Environment Variables
- **🐳 Docker Support** - Einfache Deployment-Optionen
- **📊 Logging & Monitoring** - Ausführliche Protokollierung

## 🎮 Commands Übersicht

| Command | Beschreibung |
|---------|--------------|
| `/ping` | Bot-Status überprüfen |
| `/patchnotes status` | Patch Notes Konfiguration anzeigen |
| `/patchnotes setchannel` | Channel für Patch Notes setzen |
| `/admin test-patchnotes` | Test-Benachrichtigung senden |

[Alle Commands anzeigen →](commands)

## 🔧 Environment Variables

```bash
# Erforderlich
DISCORD_TOKEN=your_bot_token
DISCORD_GUILD_ID=your_guild_id

# Optional (League Patch Notes)
LEAGUEPATCHNOTES_CHANNELID=your_channel_id
LEAGUEPATCHNOTES_ROLEID=your_role_id
```

## 🤝 Support

- **[GitHub Issues](https://github.com/ursupportermain/Ahri_Bot/issues)** - Bug Reports & Feature Requests
- **[Discussions](https://github.com/ursupportermain/Ahri_Bot/discussions)** - Community Support

## 📄 License

Dieses Projekt steht unter der [MIT License](https://github.com/ursupportermain/Ahri_Bot/blob/main/LICENSE).

---

**Entwickelt mit ❤️ für die League of Legends Community**

<style>
.grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
  margin: 20px 0;
}

.grid-item {
  border: 1px solid #e1e4e8;
  border-radius: 6px;
  padding: 16px;
  background: #f6f8fa;
}

.grid-item h3 {
  margin-top: 0;
  margin-bottom: 8px;
}

.grid-item a {
  text-decoration: none;
  color: #0366d6;
}

.grid-item a:hover {
  text-decoration: underline;
}

.grid-item p {
  margin-bottom: 0;
  color: #586069;
}
</style>
