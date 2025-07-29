# League of Legends Patch Notes Bot

Dieser Bot überwacht automatisch die offiziellen League of Legends Patch Notes und sendet Benachrichtigungen in einen konfigurierten Discord-Channel, wenn neue Patches verfügbar sind.

## 🚀 Features

- **Automatische Überwachung**: Überprüft alle 30 Minuten auf neue Patch Notes
- **Discord-Integration**: Sendet schöne Embed-Nachrichten mit Patch-Informationen
- **Rollen-Erwähnungen**: Kann optional eine Rolle erwähnen bei neuen Patches
- **Persistenz**: Speichert den letzten bekannten Patch, um Duplikate zu vermeiden
- **Slash-Commands**: Einfache Konfiguration über Discord-Befehle

## ⚙️ Konfiguration

### 1. Konfigurationsdatei

Füge die folgenden Werte zu deiner `appsettings.json` hinzu:

```json
{
  "LeaguePatchNotes": {
    "ChannelId": "1234567890123456789",
    "RoleId": "9876543210987654321"
  }
}
```

- `ChannelId`: Die Discord-Channel-ID, wo die Patch Notes gesendet werden sollen
- `RoleId`: (Optional) Die Rollen-ID, die bei neuen Patches erwähnt werden soll

### 2. User Secrets (Empfohlen für Development)

Alternativ kannst du die Konfiguration über User Secrets verwalten:

```bash
dotnet user-secrets set "LeaguePatchNotes:ChannelId" "1234567890123456789"
dotnet user-secrets set "LeaguePatchNotes:RoleId" "9876543210987654321"
```

## 🎮 Discord-Commands

### `/patchnotes status`
Zeigt die aktuelle Konfiguration an.

### `/patchnotes setchannel [channel]`
Setzt den Channel für Patch Notes Benachrichtigungen.

### `/patchnotes setrole [role]`
Setzt die Rolle, die bei neuen Patches erwähnt werden soll.

### `/patchnotes check`
Überprüft manuell auf neue Patch Notes.

## 📝 Beispiel-Benachrichtigung

```
🆕 Neue League of Legends Patch Notes!

Patch 14.15 Notes

Patch Version: 14.15
Link: Patch Notes lesen

@LoL Updates
```

## 🔧 Technische Details

- **Überwachungsintervall**: 30 Minuten
- **Quelle**: https://www.leagueoflegends.com/de-de/news/tags/patch-notes/
- **Datenformat**: HTML-Parsing mit HtmlAgilityPack
- **Persistenz**: JSON-Datei für letzten bekannten Patch

## 🛠️ Installation

1. Stelle sicher, dass die benötigten NuGet-Pakete installiert sind:
   - `HtmlAgilityPack`
   - `Microsoft.Extensions.Http`

2. Konfiguriere die Channel-ID in der `appsettings.json`

3. Starte den Bot - der Service wird automatisch gestartet

## 📊 Logging

Der Service loggt wichtige Ereignisse:
- Neue Patches gefunden
- Benachrichtigungen gesendet
- Fehler beim Abrufen der Patch Notes
- Konfigurationsänderungen

## ⚠️ Hinweise

- Der Bot benötigt Berechtigung zum Senden von Nachrichten im konfigurierten Channel
- Die erste Ausführung erkennt keinen "neuen" Patch, da noch kein Referenzpunkt existiert
- Bei Serverneustarts wird der letzte bekannte Patch aus der `last_patch.json` Datei geladen
