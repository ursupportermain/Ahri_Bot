---
layout: default
title: Docker
nav_order: 5
permalink: /docker/
---

# Docker Deployment Guide 🐳

## Local Development (Recommended: User Secrets)

### Option 1: Native .NET (Recommended for Development)
```bash
# Set up user secrets (secure, local only)
dotnet user-secrets set "Discord:Token" "your_bot_token_here"
dotnet user-secrets set "Discord:Guild:Id" "your_guild_id_here"

# Run locally
dotnet run
```

### Option 2: Docker with User Secrets
```bash
# User secrets are automatically mounted in development
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up
```

## Production Deployment

### Option 1: Railway/Heroku/Cloud Platforms
```bash
# Set environment variables in your platform's dashboard:
# DISCORD_TOKEN=your_token
# DISCORD_GUILD_ID=your_guild_id
# DOTNET_ENVIRONMENT=Production

# Deploy directly from Git repository
```

### Option 2: Docker with Environment Variables
```bash
# Run with environment variables directly
docker run -d \
  --name akali-bot \
  -e DISCORD_TOKEN="your_token" \
  -e DISCORD_GUILD_ID="your_guild_id" \
  -e DOTNET_ENVIRONMENT="Production" \
  --restart unless-stopped \
  akali-bot:latest
```

### Option 3: Docker Compose
```bash
# Set environment variables in your system or CI/CD
export DISCORD_TOKEN="your_token"
export DISCORD_GUILD_ID="your_guild_id"

# Deploy
docker-compose up -d
```

## Environment Variables

| Variable | Description | Required | Default |
|----------|-------------|----------|---------|
| `DISCORD_TOKEN` | Your Discord bot token | Yes | - |
| `DISCORD_GUILD_ID` | Your Discord server ID | No | - |
| `DOTNET_ENVIRONMENT` | Application environment | No | Production |
| `LOG_LEVEL` | Logging level | No | Information |

## Commands

### Build and Run
```bash
# Build the image
docker-compose build

# Run in background
docker-compose up -d

# Run with logs
docker-compose up

# Stop the bot
docker-compose down
```

### Development
```bash
# Run development environment
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up

# Rebuild and run
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up --build
```

### Maintenance
```bash
# View logs
docker-compose logs -f

# Restart bot
docker-compose restart akali-bot

# Update and restart
docker-compose pull && docker-compose up -d
```

## Health Checks

The bot includes health checks that verify:
- .NET runtime is available
- Container is responding

Check health status:
```bash
docker-compose ps
```

## Troubleshooting

### Bot won't start
1. Check your `.env` file has the correct Discord token
2. Verify the token has proper permissions
3. Check logs: `docker-compose logs akali-bot`

### Permission denied errors
```bash
# Fix permissions
sudo chown -R $USER:$USER ./logs
```

### Update bot
```bash
# Pull latest changes
git pull

# Rebuild and restart
docker-compose build --no-cache
docker-compose up -d
```
