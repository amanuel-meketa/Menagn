# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["security.api/security.api.csproj", "security.api/"]
COPY ["security.business/security.business.csproj", "security.business/"]
COPY ["security.data/security.data.csproj", "security.data/"]
COPY ["security.SharedUtils/security.sharedUtils.csproj", "security.SharedUtils/"]
RUN dotnet restore "./security.api/./security.api.csproj"
COPY . .
WORKDIR "/src/security.api"
RUN dotnet build "./security.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./security.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "security.api.dll"]