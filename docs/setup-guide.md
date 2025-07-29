---
layout: default
title: Setup Guide
nav_order: 2
permalink: /setup-guide/
---

# Akali Bot - Setup Guide 🛠️

Schritt-für-Schritt Anleitung zur Einrichtung des Akali Bots.

## 🚀 So richtest du die League Patch Notes Funktion ein:

### Schritt 1: Channel-ID herausfinden
1. Gehe in Discord in den Channel, wo die Patch Notes gesendet werden sollen
2. Rechtsklick auf den Channel → "Link kopieren"
3. Die Zahlen am Ende der URL sind die Channel-ID (z.B. `1234567890123456789`)

### Schritt 2: Konfiguration setzen
Öffne `appsettings.json` und füge die Channel-ID ein:

```json
{
  "LeaguePatchNotes": {
    "ChannelId": "DEINE_CHANNEL_ID_HIER",
    "RoleId": ""
  }
}
```

### Schritt 3: (Optional) Rolle für Mentions
1. Erstelle eine Rolle für League-Updates (z.B. "@LoL Updates")
2. Rechtsklick auf die Rolle → "ID kopieren" (Developer Mode muss aktiviert sein)
3. Füge die Rollen-ID in die Konfiguration ein:

```json
{
  "LeaguePatchNotes": {
    "ChannelId": "DEINE_CHANNEL_ID_HIER",
    "RoleId": "DEINE_ROLLEN_ID_HIER"
  }
}
```

### Schritt 4: Bot starten
```bash
dotnet run
```

## 🎮 Commands zum Testen:

- `/patchnotes status` - Zeigt die aktuelle Konfiguration
- `/patchnotes check` - Überprüft manuell auf neue Patches
- `/admin test-patchnotes` - Sendet eine Test-Benachrichtigung (nur Admins)
- `/admin service-status` - Zeigt Service-Status (nur Admins)

## ⚠️ Wichtige Hinweise:

1. Der Bot braucht `Send Messages` und `Embed Links` Berechtigung im Ziel-Channel
2. Beim ersten Start wird kein "neuer" Patch erkannt (das ist normal)
3. Der Bot überprüft automatisch alle 30 Minuten auf Updates
4. Developer Mode in Discord aktivieren für ID-Kopieren: Einstellungen → Erweitert → Entwicklermodus

## 🐛 Troubleshooting:

**Bot sendet keine Nachrichten:**
- Überprüfe Channel-ID in der Konfiguration
- Stelle sicher, dass der Bot Berechtigung im Channel hat
- Schaue in die Logs für Fehlermeldungen

**"Channel nicht gefunden" Fehler:**
- Channel-ID ist falsch oder Channel wurde gelöscht
- Bot ist nicht auf dem richtigen Server

**Rolle wird nicht erwähnt:**
- Rollen-ID ist falsch
- Rolle wurde gelöscht oder umbenannt
