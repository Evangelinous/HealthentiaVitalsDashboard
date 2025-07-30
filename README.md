
# HealthentiaVitalsDashboard

A real-time Patient Vital Signs Monitoring Dashboard built with **ASP.NET Core**, **SignalR**, and **Razor Pages**. This project displays patient data such as heart rate, temperature, and oxygen saturation, and updates them in real time.

---

## Project Idea

The dashboard simulates real-time health monitoring, making it useful for:
- Demonstrations of patient telemetry in hospitals.
- Educational purposes in biomedical and software engineering.
- Prototypes for IoT-based healthcare platforms.

---

## Technologies Used

- **.NET 8** with ASP.NET Core Web App (Razor Pages)
- **SignalR** for real-time updates
- **Entity Framework Core** with **SQLite** as database
- **BackgroundService** for simulating vital updates
- **xUnit** for unit testing
- **Docker** for containerization

---
##  Clone it locally with Git:

git clone https://github.com/Evangelinous/HealthentiaVitalsDashboard.git

---

## Project Structure

```
HealthentiaVitalsDashboard/
├── Controllers/
├── Data/
├── Hubs/
├── Models/
├── Pages/
├── Services/
├── app.db                # SQLite database
├── Program.cs
├── Startup.cs (if present)
├── HealthentiaVitalsDashboard.csproj
├── Dockerfile
HealthentiaVitalsDashboard.Tests/
├── ...                   # xUnit test project
HealthentiaVitalsDashboard.sln
```

---

## Run the project locally

### Prerequisites
- .NET 8 SDK
- SQLite (optional – DB is auto-created)
- Docker (for containerized run)

### 1. Run Locally (without Docker)
```bash

dotnet run --project HealthentiaVitalsDashboard/HealthentiaVitalsDashboard.csproj

OR go inside the project and 

cd HealthentiaVitalsDashboard
dotnet build
dotnet run
```

The app will run on:  
[http://localhost:5228](http://localhost:5228)

Available Swagger APIs

The application exposes the following documented APIs (via Swagger):

Auth
	•	POST /api/Auth/login: Authenticate using email/password → returns JWT token.

Patient Vitals
	•	POST /patientvitals/{id}/vitals: Submits a new vital sign record for a specific patient.

Each endpoint is secured and tested. To interact, first Authorize using your JWT from /api/Auth/login.

You can test and explore all APIs using Swagger here:
	•	http://localhost:5228/swagger (local)
	•	http://localhost:5050/swagger (Docker)

    
---

### 2. Run via Docker

#### Build the Docker image:
```bash
docker build -t healthentia-dashboard -f HealthentiaVitalsDashboard/Dockerfile .
```

#### Run the container:
```bash
docker run -p 5050:5050 healthentia-dashboard
```

The app will now be accessible at:  
[http://localhost:5050](http://localhost:5050)

---

## Run Unit Tests

You can run unit tests directly from the command line:

```bash
dotnet test HealthentiaVitalsDashboardTests/HealthentiaVitalsDashboard.Tests.csproj
```

Or as part of the Docker build (already included in the Dockerfile).

---

## Notes

- The app includes a pre-seeded list of patients and simulates live vital changes.
- SignalR pushes these changes to the frontend in real time.
- Data is displayed in tables and optionally via charts.

---

## License

This project is for educational and demo purposes.
