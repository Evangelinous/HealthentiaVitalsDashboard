# Use the ASP.NET runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln ./
COPY HealthentiaVitalsDashboard.csproj ./

# Restore dependencies
RUN dotnet restore

# Copy everything and build
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Copy the SQLite database to container
COPY app.db ./

ENTRYPOINT ["dotnet", "HealthentiaVitalsDashboard.dll"]

ENV ASPNETCORE_URLS=http://0.0.0.0:5000