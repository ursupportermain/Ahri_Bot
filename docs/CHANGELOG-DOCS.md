# Changelog - README & Documentation Reorganization

## 📋 Änderungen

### ✅ Was wurde gemacht:

1. **README.md komplett überarbeitet**
   - Moderne, übersichtliche Struktur mit Emojis
   - Fokus auf wesentliche Informationen
   - Quick Start Sektion hinzugefügt
   - Badges für Technologie-Stack
   - Klare Command-Übersicht
   - Projektstruktur visualisiert

2. **Dokumentation reorganisiert**
   - Neuer `docs/` Ordner erstellt
   - Alle Markdown-Dateien dorthin verschoben:
     - `DEPLOYMENT.md` → `docs/DEPLOYMENT.md`
     - `DOCKER.md` → `docs/DOCKER.md`  
     - `LEAGUE_PATCH_NOTES.md` → `docs/LEAGUE_PATCH_NOTES.md`
     - `SETUP_GUIDE.md` → `docs/SETUP_GUIDE.md`

3. **GitHub Wiki Vorbereitung**
   - `docs/wiki-home.md` - Wiki Startseite erstellt
   - `docs/wiki-setup.md` - Anleitung für Wiki-Setup
   - Links in README auf `docs/` Ordner aktualisiert

### 🎯 Vorteile:

- **Saubere Repository-Struktur** - Weniger Clutter im Root
- **GitHub Wiki Ready** - Alle Dateien bereit für Wiki-Import
- **Professionelles Erscheinungsbild** - Moderne README mit Badges
- **Bessere Navigation** - Klare Verlinkung zur Dokumentation
- **Wartbarkeit** - Dokumentation zentral im `docs/` Ordner

### 📁 Neue Struktur:

```
Ahri_Bot/
├── 📄 README.md              # Überarbeitete Hauptdokumentation
├── 📁 docs/                  # Alle Dokumentation
│   ├── 📄 wiki-home.md       # GitHub Wiki Startseite
│   ├── 📄 wiki-setup.md      # Wiki Setup Anleitung
│   ├── 📄 SETUP_GUIDE.md     # Einrichtungsanleitung
│   ├── 📄 LEAGUE_PATCH_NOTES.md # Patch Notes Feature
│   ├── 📄 DOCKER.md          # Docker Deployment
│   └── 📄 DEPLOYMENT.md      # Deployment Guide
├── 📁 Ahri.Core/            # Bot Code
└── ...
```

### 🚀 Nächste Schritte für GitHub Wiki:

1. Repository → Settings → Features → Wikis ✅ aktivieren
2. Wiki-Seiten aus `docs/` Ordner erstellen
3. Navigation/Sidebar konfigurieren
4. Optional: GitHub Actions für Auto-Sync

Die README ist jetzt viel übersichtlicher und fokussiert auf das Wesentliche, während alle detaillierten Informationen sauber im `docs/` Ordner organisiert sind! 🎉
