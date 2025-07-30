# 🔊 Voice Channel System

Das Voice Channel System ermöglicht es Benutzern, temporäre Voice Channels zu erstellen und zu verwalten.

## 🚀 Setup (Admin)

1. Verwende `/voice setup` um das System einzurichten
2. Gib einen Kategorienamen ein (z.B. "Voice Channels")
3. Gib einen Channel-Namen ein (z.B. "Join to Create")

## 👤 Benutzer Commands

### Grundlegende Befehle
- `/voice help` - Zeigt alle verfügbaren Commands
- `/voice lock` - Sperrt deinen Channel
- `/voice unlock` - Entsperrt deinen Channel
- `/voice claim` - Übernimmt einen Channel wenn der Owner weg ist

### Channel Anpassung
- `/voice name <name>` - Ändert den Namen deines Channels
- `/voice limit <zahl>` - Setzt das Benutzerlimit (0 = unbegrenzt)

### Benutzer Management
- `/voice permit @user` - Erlaubt einem Benutzer den Zugang
- `/voice reject @user` - Verweigert einem Benutzer den Zugang

## 🔧 Admin Commands

### Voice Management
- `/admin voice-stats` - Zeigt Voice Channel Statistiken
- `/admin voice-cleanup` - Bereinigt verwaiste Einträge
- `/admin set-voice-limit <limit>` - Setzt Standard-Limit für Server
- `/admin force-delete-voice <channel>` - Löscht einen Voice Channel gewaltsam

## 🎯 Funktionsweise

1. **Channel Erstellung**: Benutzer joinen den "Join to Create" Channel
2. **Automatische Erstellung**: Ein neuer temporärer Channel wird erstellt
3. **Benutzer wird bewegt**: Automatischer Move in den neuen Channel
4. **Channel Management**: Owner kann Channel anpassen
5. **Automatische Löschung**: Channel wird gelöscht wenn leer

## 📊 Features

### ✅ Implementiert
- [x] Automatische Channel-Erstellung
- [x] Channel-Berechtigungen verwalten
- [x] Benutzer-spezifische Einstellungen
- [x] Server-weite Standardeinstellungen
- [x] Cooldown-System (15 Sekunden)
- [x] Automatische Channel-Löschung
- [x] Channel Ownership Transfer
- [x] Admin-Übersicht und Management

### 🎨 Benutzerfreundlichkeit
- Deutsche Benutzeroberfläche
- Detaillierte Fehlermeldungen
- Ephemeral Responses für Private Commands
- Konsistente Embed-Designs
- Ausführliches Logging

## 🗃️ Datenbank

Das System verwendet eine SQLite-Datenbank (`voice.db`) mit folgenden Tabellen:

- **guild**: Server-Konfiguration
- **voiceChannel**: Aktive Voice Channels
- **userSettings**: Benutzer-Einstellungen
- **guildSettings**: Server-Standardeinstellungen

## 🔒 Berechtigungen

- **Setup**: Administrator-Berechtigung erforderlich
- **Commands**: Jeder Benutzer kann seine eigenen Channels verwalten
- **Admin Commands**: Administrator-Berechtigung erforderlich

## 📝 Logging

Alle wichtigen Aktionen werden geloggt:
- Channel-Erstellung und -Löschung
- Berechtigungsänderungen
- Admin-Aktionen
- Fehler und Exceptions
