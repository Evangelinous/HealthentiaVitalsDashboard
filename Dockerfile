# Stage 1: Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Stage 2: Build and test
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY HealthentiaVitalsDashboard/HealthentiaVitalsDashboard.sln ./HealthentiaVitalsDashboard/
COPY HealthentiaVitalsDashboard/HealthentiaVitalsDashboard.csproj ./HealthentiaVitalsDashboard/
COPY HealthentiaVitalsDashboardTests/HealthentiaVitalsDashboard.Tests.csproj ./HealthentiaVitalsDashboardTests/

# Restore dependencies
RUN dotnet restore HealthentiaVitalsDashboard/HealthentiaVitalsDashboard.sln

# Copy the rest of the source code
COPY HealthentiaVitalsDashboard/ ./HealthentiaVitalsDashboard/
COPY HealthentiaVitalsDashboardTests/ ./HealthentiaVitalsDashboardTests/

# Run unit tests
RUN dotnet test HealthentiaVitalsDashboardTests/HealthentiaVitalsDashboard.Tests.csproj --no-restore --verbosity normal

# Publish the main app
RUN dotnet publish HealthentiaVitalsDashboard/HealthentiaVitalsDashboard.csproj -c Release -o /app/publish

# Stage 3: Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY HealthentiaVitalsDashboard/app.db ./
ENTRYPOINT ["dotnet", "HealthentiaVitalsDashboard.dll"]

EXPOSE 5050
ENV ASPNETCORE_URLS=http://0.0.0.0:5050