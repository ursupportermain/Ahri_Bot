# Dockerfile für den Discord Bot
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Akali.Core/Akali.Core.csproj", "Akali.Core/"]
RUN dotnet restore "Akali.Core/Akali.Core.csproj"
COPY . .
WORKDIR "/src/Akali.Core"
RUN dotnet build "Akali.Core.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Akali.Core.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Akali.Core.dll"]
