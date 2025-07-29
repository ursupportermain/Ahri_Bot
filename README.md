# Akali Bot

A Discord bot built with .NET 9 and Discord.Net.

## Setup

### 1. Prerequisites
- .NET 9 SDK
- A Discord Bot Token (from Discord Developer Portal)

### 2. Configuration

Das Projekt unterstützt verschiedene Konfigurationsmethoden für verschiedene Umgebungen:

#### 🔧 **Local Development (Empfohlen: User Secrets)**

```bash
cd Akali.Core

# Sicher: User Secrets (nicht in Git committed)
dotnet user-secrets set "Discord:Token" "YOUR_BOT_TOKEN_HERE"
dotnet user-secrets set "Discord:Guild:Id" "YOUR_GUILD_ID_HERE"
```

#### 🚀 **Production Deployment**

**Option 1: Railway**
- Konfiguriere Environment Variables im Railway Dashboard:
  - `DISCORD_TOKEN` = your_bot_token
  - `DISCORD_GUILD_ID` = your_guild_id
  - `DOTNET_ENVIRONMENT` = Production

**Option 2: GitHub Actions**
- Setze Repository Secrets unter Settings > Secrets:
  - `DISCORD_TOKEN` = your_bot_token  
  - `DISCORD_GUILD_ID` = your_guild_id

**Option 3: Docker Compose (VPS/Self-hosted)**
```bash
cp .env.example .env
nano .env  # Füge deine Production-Secrets hinzu
```

#### 🔄 **Konfigurationspriorität:**
1. **Environment Variables** (Production)
2. **User Secrets** (Development) 
3. **appsettings.json** (Fallback/Default)

### 3. Konfigurationsdateien

- `appsettings.json` - Basis-Konfiguration
- `appsettings.Development.json` - Development-spezifische Einstellungen
- `Properties/launchSettings.json` - Launch-Profile für verschiedene Umgebungen
- `.gitignore` - Git-Ignore-Regeln für .NET Projekte

### 4. Running the Bot

#### 🔧 **Development**
```bash
# Restore and build
dotnet restore
dotnet build

# Run locally
dotnet run
```

#### 🚀 **Production**
See detailed deployment guides:
- **[DEPLOYMENT.md](DEPLOYMENT.md)** - Complete deployment guide for all platforms
- **[DOCKER.md](DOCKER.md)** - Docker-specific instructions

**Quick Railway deployment:**
1. Fork this repo
2. Connect to Railway  
3. Set `DISCORD_TOKEN` environment variable
4. Deploy automatically

**Quick Docker deployment:**
```bash
cp .env.example .env  # Add your token
docker-compose up -d
```

## Project Structure

- `Program.cs` - Application entry point and service configuration
- `Services/DiscordService.cs` - Main Discord service handling bot lifecycle
- `Commands/` - Slash command implementations
- `Akali.Core.csproj` - Project configuration with user secrets support
- `appsettings.json` - Base configuration file
- `appsettings.Development.json` - Development-specific settings
- `Properties/launchSettings.json` - Launch profiles for different environments
- `Dockerfile` - Docker container configuration
- `docker-compose.yml` - Docker compose for production
- `docker-compose.dev.yml` - Docker compose for development  
- `railway.json` - Railway deployment configuration
- `.github/workflows/` - CI/CD pipelines
- `.gitignore` - Git ignore rules for .NET projects
- `LICENSE` - MIT License file
- `DOCKER.md` - Docker deployment guide
- `DEPLOYMENT.md` - Complete deployment guide

## Features

- Slash command support
- Global and guild-specific command registration
- Proper logging and error handling
- Background service architecture
- Secure token management via user secrets
- Multiple configuration methods (User Secrets, Environment Variables, appsettings)
- Development and Production profiles

## Discord Bot Permissions

Make sure your Discord bot has the following permissions:
- Send Messages
- Use Slash Commands
- Read Message History
- View Channels

And the following Gateway Intents:
- Guilds
- Guild Messages  
- Message Content
- Guild Members

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
