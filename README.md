# Healthentia Vitals Dashboard

A real-time ASP.NET Core web application for monitoring patient vital signs using SignalR, Razor Pages, and SQLite.

## 🚀 Features

- 📈 Real-time charts for Heart Rate, Blood Pressure & Oxygen Saturation (Chart.js)
- 🩺 Patient vital data simulated and updated via background service
- ✅ SignalR hub pushes live updates to all connected clients
- 📊 Dynamic row highlighting based on vital status (Normal, Warning, Critical)
- 🔒 User registration and login via ASP.NET Core Identity
- 📦 Export patient vitals to CSV

---

## 🛠 Technologies

- ASP.NET Core 8.0 (Razor Pages)
- Entity Framework Core + SQLite
- SignalR (real-time communication)
- Chart.js (frontend chart rendering)
- Bootstrap 5 (styling)
- xUnit + Coverlet (unit testing & code coverage)

---

## 🧪 Testing

Unit tests cover all business logic including status evaluation.

### Run Tests

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## 📦 Running the Project

### 1. Restore packages

```bash
dotnet restore
```

### 2. Apply migrations (if needed)

```bash
dotnet ef database update
```

### 3. Run the app

```bash
dotnet run
```

App will be available at `https://localhost:PORT`.

---

## 🐳 Docker

You can also run the app via Docker:

```bash
docker build -t healthentia-dashboard .
docker run -p 5050:5050 healthentia-dashboard
```

Then visit: [http://localhost:5050](http://localhost:5050)

---

## 📁 Project Structure

```
├── Areas/Identity/Pages/Account        # Identity UI (Register/Login)
├── Controllers/                        # REST API controllers
├── Hubs/                               # SignalR hub
├── Models/                             # Entity models
├── Services/                           # Business logic services
├── Views/                              # Razor views
├── wwwroot/                            # Static files
├── Data/                               # EF Core DbContext
├── appsettings.json                    # Configuration
```

---

## 📄 License

This project is licensed under the MIT License.
