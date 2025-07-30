# Ahri Bot Documentation Site

Diese Website wird automatisch mit GitHub Pages aus dem `docs/` Ordner generiert.

## 🌐 Live Site

**URL:** [https://ursupportermain.github.io/Ahri_Bot/](https://ursupportermain.github.io/Ahri_Bot/)

## 🏗️ Technologie

- **Jekyll** - Static Site Generator
- **GitHub Pages** - Hosting
- **Cayman Theme** - Responsive Design
- **GitHub Actions** - Automatisches Deployment

## 📁 Struktur

```
docs/
├── _config.yml           # Jekyll Konfiguration
├── _layouts/
│   └── default.html      # Haupt-Layout
├── index.md              # Startseite
├── setup-guide.md        # Setup Anleitung
├── league-patch-notes.md # Patch Notes Feature
├── commands.md           # Bot Commands
├── docker.md             # Docker Deployment
├── deployment.md         # Deployment Guide
└── README.md            # Diese Datei
```

## 🔧 Lokale Entwicklung

### Mit Jekyll (Empfohlen)

```bash
# Jekyll installieren
gem install bundler jekyll

# In docs Ordner wechseln
cd docs

# Dependencies installieren
bundle install

# Lokal starten
bundle exec jekyll serve --watch

# Website öffnen
open http://localhost:4000/Ahri_Bot/
```

### Mit Docker

```bash
# In docs Ordner wechseln
cd docs

# Jekyll Container starten
docker run --rm -it \
  -p 4000:4000 \
  -v "$(pwd):/srv/jekyll" \
  jekyll/jekyll:latest \
  jekyll serve --watch --force_polling

# Website öffnen
open http://localhost:4000/Ahri_Bot/
```

## 📝 Inhalte bearbeiten

### Front Matter
Jede Seite benötigt Front Matter am Anfang:

```yaml
---
layout: default
title: Seitentitel
nav_order: 1
permalink: /url-pfad/
---
```

### Navigation
Navigation wird in `_config.yml` konfiguriert:

```yaml
nav_links:
  - title: Home
    url: /
  - title: Setup Guide
    url: /setup-guide
```

### Markdown Features
- Standard Markdown wird unterstützt
- Syntax Highlighting für Code-Blöcke
- Emoji-Support 🎉
- Tabellen und Listen
- Custom CSS in `_layouts/default.html`

## 🚀 Deployment

### Automatisch (GitHub Actions)
- Läuft automatisch bei Push auf `main` Branch
- Deployment-Status: [![Pages](https://github.com/ursupportermain/Ahri_Bot/actions/workflows/pages.yml/badge.svg)](https://github.com/ursupportermain/Ahri_Bot/actions/workflows/pages.yml)

### Manuell
1. Repository Settings → Pages
2. Source: GitHub Actions
3. Branch: main
4. Folder: docs

## 🎨 Customization

### Theme anpassen
- Layout in `_layouts/default.html`
- CSS direkt im Layout oder in separater Datei
- Variablen in `_config.yml`

### Neue Seiten hinzufügen
1. Neue `.md` Datei in `docs/` erstellen
2. Front Matter hinzufügen
3. In `_config.yml` Navigation aktualisieren
4. Commit & Push

## 🔍 SEO & Metadata

- Title und Description in `_config.yml`
- Per-Page Metadata im Front Matter
- Open Graph Meta Tags
- Sitemap wird automatisch generiert

## 📊 Analytics (Optional)

Google Analytics kann in `_config.yml` konfiguriert werden:

```yaml
google_analytics: UA-XXXXXXXXX-X
```

## 🛠️ Troubleshooting

**Site wird nicht aktualisiert:**
- Check GitHub Actions Tab für Deployment-Status
- Warte 5-10 Minuten nach Push
- Hard Refresh im Browser (Ctrl+F5)

**Jekyll Build Fehler:**
- Überprüfe YAML Front Matter Syntax
- Validiere `_config.yml`
- Check Actions Logs für Details

**Broken Links:**
- Verwende relative URLs mit `{{ "/path/" | relative_url }}`
- Teste lokal vor dem Push
