# Deployment Guide

## 🏠 Local Development

### Recommended: User Secrets
```bash
cd Akali.Core

# Set bot token (secure, not committed to git)
dotnet user-secrets set "Discord:Token" "your_bot_token_here"
dotnet user-secrets set "Discord:Guild:Id" "your_guild_id_here"

# Run the bot
dotnet run
```

### Alternative: Docker with User Secrets
```bash
# Development docker setup (mounts user secrets)
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up
```

---

## 🚀 Production Deployment

### Option 1: Railway ⭐ (Recommended)

1. **Fork/Clone** this repository
2. **Connect** to Railway
3. **Configure Environment Variables** in Railway dashboard:
   ```
   DISCORD_TOKEN=your_discord_bot_token
   DISCORD_GUILD_ID=your_guild_id (optional)
   DOTNET_ENVIRONMENT=Production
   ```
4. **Deploy** automatically via Railway

**Railway Benefits:**
- ✅ Automatic deployments from GitHub
- ✅ Free tier available
- ✅ Easy environment variable management
- ✅ Automatic container builds

### Option 2: GitHub Actions + Container Registry

1. **Set Repository Secrets** (Settings > Secrets and variables > Actions):
   ```
   DISCORD_TOKEN=your_discord_bot_token
   DISCORD_GUILD_ID=your_guild_id
   ```

2. **Push to main branch** - CI/CD pipeline runs automatically:
   - Builds and tests the bot
   - Creates Docker image
   - Publishes to GitHub Container Registry

3. **Deploy** the published image to your hosting provider

### Option 3: Docker Compose (VPS/Self-hosted)

1. **Clone** repository on your server:
   ```bash
   git clone https://github.com/ursupportermain/Akali_Bot.git
   cd Akali_Bot
   ```

2. **Set** environment variables:
   ```bash
   export DISCORD_TOKEN="your_discord_bot_token"
   export DISCORD_GUILD_ID="your_guild_id"
   ```

3. **Deploy**:
   ```bash
   docker-compose up -d
   ```

4. **Monitor** logs:
   ```bash
   docker-compose logs -f akali-bot
   ```

### Option 4: Manual Docker Build

```bash
# Build image
docker build -t akali-bot .

# Run container
docker run -d \
  --name akali-bot \
  -e DISCORD_TOKEN="your_token" \
  -e DISCORD_GUILD_ID="your_guild_id" \
  -e DOTNET_ENVIRONMENT="Production" \
  --restart unless-stopped \
  akali-bot
```

---

## 🔧 Configuration Overview

| Environment | Method | Security | Complexity |
|-------------|--------|----------|------------|
| **Development** | User Secrets | 🟢 High | 🟢 Low |
| **Railway** | Environment Variables | 🟡 Medium | 🟢 Low |
| **GitHub Actions** | Repository Secrets | 🟢 High | 🟡 Medium |
| **Docker Compose** | .env file | 🔴 Low | 🟡 Medium |
| **Manual Docker** | CLI Arguments | 🔴 Low | 🔴 High |

---

## 🛠 Troubleshooting

### Bot doesn't start
1. **Check token** - Verify it's valid and has correct permissions
2. **Check logs** - Look for error messages
3. **Check intents** - Ensure bot has required Discord intents

### Permission errors (Docker)
```bash
sudo chown -R $USER:$USER ./logs
```

### Update deployment
```bash
# Railway: Push to main branch (auto-deploys)
git push origin main

# Docker Compose: Pull and restart
git pull && docker-compose up --build -d
```

---

## 📊 Monitoring

### Check bot status
```bash
# Docker Compose
docker-compose ps
docker-compose logs -f akali-bot

# Railway
# Check logs in Railway dashboard

# Manual Docker
docker ps
docker logs akali-bot -f
```

### Health checks
The bot includes health checks that verify:
- .NET runtime is working
- Container is responding
- Discord connection is active
