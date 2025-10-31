FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["src/Azure.SignalR.HubApi/", "Azure.SignalR.HubApi/"]
COPY ["Directory.Build.props", "Directory.Build.props"] 
COPY ["Directory.Packages.props", "Directory.Packages.props"]

RUN dotnet restore Azure.SignalR.HubApi/Azure.SignalR.HubApi.csproj
RUN dotnet build Azure.SignalR.HubApi/Azure.SignalR.HubApi.csproj -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish Azure.SignalR.HubApi/Azure.SignalR.HubApi.csproj \
    -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Azure.SignalR.HubApi.dll"]
