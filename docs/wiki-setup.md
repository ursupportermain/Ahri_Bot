# GitHub Wiki Setup

Dieses Repository enthält alle Dateien für das GitHub Wiki im `docs/` Ordner.

## Wiki Seiten Setup

### 1. Wiki aktivieren
1. Gehe zu deinem GitHub Repository
2. Settings → Features → Wikis ✅ aktivieren

### 2. Wiki Seiten erstellen

Erstelle die folgenden Wiki-Seiten mit den entsprechenden Inhalten:

| Wiki Seite | Datei | Beschreibung |
|------------|-------|--------------|
| **Home** | `wiki-home.md` | Wiki Startseite |
| **Setup-Guide** | `SETUP_GUIDE.md` | Einrichtungsanleitung |
| **League-Patch-Notes** | `LEAGUE_PATCH_NOTES.md` | Patch Notes Feature |
| **Docker** | `DOCKER.md` | Docker Deployment |
| **Deployment** | `DEPLOYMENT.md` | Vollständige Deployment-Anleitung |

### 3. Wiki Navigation

Empfohlene Sidebar (`_Sidebar.md`):
```markdown
**📚 Ahri Bot Wiki**

**🚀 Schnellstart**
- [Setup Guide](Setup-Guide)
- [Quick Start](https://github.com/ursupportermain/Ahri_Bot#-quick-start)

**📖 Anleitungen**
- [League Patch Notes](League-Patch-Notes)
- [Docker Deployment](Docker)
- [Deployment Guide](Deployment)

**🔗 Links**
- [Repository](https://github.com/ursupportermain/Ahri_Bot)
- [Issues](https://github.com/ursupportermain/Ahri_Bot/issues)
```

### 4. Automatische Synchronisation (Optional)

Du kannst GitHub Actions einrichten, um das Wiki automatisch zu aktualisieren:

```yaml
# .github/workflows/sync-wiki.yml
name: Sync Wiki
on:
  push:
    paths:
      - 'docs/**'
    branches:
      - main

jobs:
  sync-wiki:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout repository
        uses: actions/checkout@v4
        
      - name: Sync Wiki
        uses: newrelic/wiki-sync-action@main
        with:
          source: docs/
          destination: wiki/
          token: ${{ secrets.GITHUB_TOKEN }}
```

## 💡 Wiki Best Practices

1. **Konsistente Namensgebung** - Verwende Kebab-Case für Wiki-Seiten
2. **Interne Links** - Verwende relative Links zwischen Wiki-Seiten  
3. **Regelmäßige Updates** - Halte die Dokumentation aktuell
4. **Bilder/Assets** - Speichere sie im Repository und verlinke sie

## 🔄 Workflow

1. **Dokumentation bearbeiten** in `docs/` Ordner
2. **Commit & Push** zu GitHub
3. **Wiki manuell aktualisieren** oder automatisch synchronisieren
4. **Links testen** und sicherstellen, dass alles funktioniert
