# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy everything first (since context is monorepo root)
COPY . .

# Navigate to the API project and restore
WORKDIR /src/apps/userservice/UserService
RUN dotnet restore "Waggle.UserService.csproj"

# Build from the API directory
RUN dotnet build "Waggle.UserService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/apps/userservice/UserService
RUN dotnet publish "Waggle.UserService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Critical: Set the URLs for Docker
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "Waggle.UserService.dll"]