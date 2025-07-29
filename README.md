# Akali Bot

A Discord bot built with .NET 9 and Discord.Net.

## Setup

### 1. Prerequisites
- .NET 9 SDK
- A Discord Bot Token (from Discord Developer Portal)

### 2. Configuration

Das Projekt unterstützt mehrere Konfigurationsmethoden in folgender Prioritätsreihenfolge:
1. User Secrets (empfohlen für Development)
2. Umgebungsvariablen
3. appsettings.json Dateien

#### Methode 1: User Secrets (Empfohlen für Development)

1. Navigate to the project directory:
   ```bash
   cd Akali.Core
   ```

2. Set your Discord bot token using user secrets:
   ```bash
   dotnet user-secrets set "Discord:Token" "TOKEN"
   ```

3. Optional: Ändere die Guild ID:
   ```bash
   dotnet user-secrets set "Discord:Guild:Id" "ID"
   ```

#### Methode 2: Appsettings.json mit secrets

Bearbeite die `appsettings.json` oder `appsettings.Development.json`:
```json
{
  "Discord": {
    "Token": "TOKEN as secret",
    "Guild": "ID as secret"
  }
}
```

### 3. Konfigurationsdateien

- `appsettings.json` - Basis-Konfiguration
- `appsettings.Development.json` - Development-spezifische Einstellungen
- `Properties/launchSettings.json` - Launch-Profile für verschiedene Umgebungen
- `.gitignore` - Git-Ignore-Regeln für .NET Projekte

### 4. Running the Bot

1. Restore dependencies:
   ```bash
   dotnet restore
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run the bot:
   ```bash
   # Development mode
   dotnet run
   
   # Production mode
   dotnet run --launch-profile Production
   ```

## Project Structure

- `Program.cs` - Application entry point and service configuration
- `Services/DiscordService.cs` - Main Discord service handling bot lifecycle
- `Akali.Core.csproj` - Project configuration with user secrets support
- `appsettings.json` - Base configuration file
- `appsettings.Development.json` - Development-specific settings
- `Properties/launchSettings.json` - Launch profiles for different environments
- `.gitignore` - Git ignore rules for .NET projects
- `LICENSE` - MIT License file

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
