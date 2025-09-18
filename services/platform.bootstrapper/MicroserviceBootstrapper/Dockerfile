# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MicroserviceBootstrapper/MicroserviceBootstrapper.csproj", "MicroserviceBootstrapper/"]
RUN dotnet restore "MicroserviceBootstrapper/MicroserviceBootstrapper.csproj"
COPY . .
WORKDIR "/src/MicroserviceBootstrapper"
RUN dotnet publish "MicroserviceBootstrapper.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS final
WORKDIR /app

# Add a non-root user
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MicroserviceBootstrapper.dll"]
