# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["approvals.api/approvals.api.csproj", "approvals.api/"]
COPY ["approvals.application/approvals.application.csproj", "approvals.application/"]
COPY ["approvals.domain/approvals.domain.csproj", "approvals.domain/"]
COPY ["approvals.infrastructure/approvals.infrastructure.csproj", "approvals.infrastructure/"]
RUN dotnet restore "./approvals.api/./approvals.api.csproj"
COPY . .
WORKDIR "/src/approvals.api"
RUN dotnet build "./approvals.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./approvals.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "approvals.api.dll"]