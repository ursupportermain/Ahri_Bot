---
layout: default
title: Commands
nav_order: 4
permalink: /commands/
---

# Bot Commands Übersicht ⚡

## 🎮 Alle verfügbaren Commands

### Allgemeine Commands

| Command | Beschreibung | Permissions |
|---------|--------------|-------------|
| `/ping` | Überprüft ob der Bot online und erreichbar ist | Jeder |

### League of Legends Features

| Command | Beschreibung | Permissions |
|---------|--------------|-------------|
| `/patchnotes status` | Zeigt die aktuelle Patch Notes Konfiguration | Jeder |
| `/patchnotes setchannel [channel]` | Setzt den Channel für Patch Notes Benachrichtigungen | Jeder |
| `/patchnotes setrole [role]` | Setzt die Rolle die bei neuen Patches erwähnt wird | Jeder |
| `/patchnotes check` | Überprüft manuell auf neue League Patch Notes | Jeder |

### Admin Commands

| Command | Beschreibung | Permissions |
|---------|--------------|-------------|
| `/admin test-patchnotes` | Sendet eine Test Patch Notes Benachrichtigung | Administrator |
| `/admin service-status` | Zeigt den Status aller Bot Services an | Administrator |

## 💡 Command Details

### `/ping`
- **Zweck**: Bot-Verfügbarkeit testen
- **Antwort**: Embed mit "Pong!" und Online-Status
- **Verwendung**: Debugging, Status-Check

### `/patchnotes` Commands
- **Automatik**: Bot prüft alle 30 Minuten auf neue Patches
- **Benachrichtigung**: Schöne Embeds mit Patch-Informationen
- **Mentions**: Optional Rollen-Erwähnungen
- **Persistenz**: Verhindert Duplikate durch Patch-Tracking

### Admin Commands
- **Berechtigung**: Erfordert "Administrator" Permission
- **Zweck**: Bot-Verwaltung und Diagnostik
- **Sicherheit**: Nur für vertrauenswürdige Server-Admins

## 🔧 Command Syntax

```bash
# Basis Commands
/ping

# Patch Notes Management
/patchnotes status
/patchnotes setchannel channel:#patch-notes
/patchnotes setrole role:@LoL Updates
/patchnotes check

# Admin Tools
/admin test-patchnotes
/admin service-status
```

## 📝 Hinweise

- Alle Commands sind **Slash Commands** (mit `/`)
- Commands unterstützen **Auto-Complete** 
- Fehler werden als **ephemeral messages** (nur für User sichtbar) angezeigt
- Admin Commands sind **permission-gated**
- Bot benötigt entsprechende **Channel-Permissions**
