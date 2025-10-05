# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["platform.authorization/authorization.api.csproj", "platform.authorization/"]
COPY ["authorization.application/authorization.application.csproj", "authorization.application/"]
COPY ["authorization.data/authorization.data.csproj", "authorization.data/"]
COPY ["authorization.infrastructure/authorization.infrastructure.csproj", "authorization.infrastructure/"]
RUN dotnet restore "./platform.authorization/authorization.api.csproj"
COPY . .
WORKDIR "/src/platform.authorization"
RUN dotnet build "./authorization.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./authorization.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "authorization.api.dll"]