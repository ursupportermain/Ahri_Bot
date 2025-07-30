# Dockerfile für den Discord Bot
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file first for better layer caching
COPY ["Ahri.Core/Ahri.Core.csproj", "Ahri.Core/"]
RUN dotnet restore "Ahri.Core/Ahri.Core.csproj"

# Copy all source files
COPY Ahri.Core/ Ahri.Core/
WORKDIR "/src/Ahri.Core"

# Build the application
RUN dotnet build "Ahri.Core.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ahri.Core.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ahri.Core.dll"]
