# GameStore7 API

A RESTful API built with ASP.NET Core and Entity Framework Core for managing a game store.

## Tech Stack
- ASP.NET Core (.NET 10)
- Entity Framework Core
- SQL Server (Docker)
- JWT Authentication

---

## Prerequisites
Make sure you have the following installed:
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

---

## Getting Started

### Step 1 — Clone the repository
```powershell
git clone https://github.com/CHARAN-DUSA/C-Sharp.git
cd C-Sharp/GameStore7/GameStore.Api
```

### Step 2 — Start SQL Server
```powershell
$sa_password = "Your password here"
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=$sa_password" -p 1433:1433 -v sqlvolume:/var/opt/mssql -d --rm --name mssql mcr.microsoft.com/mssql/server:2022-latest
```

### Step 3 — Set the connection string
```powershell
dotnet user-secrets set "ConnectionStrings:GameStoreContext" "Server=localhost,1433;Database=GameStore;User Id=sa;Password=$sa_password;TrustServerCertificate=True;Encrypt=False"
```

### Step 4 — Run the API
```powershell
dotnet run
```

The API will be available at `http://localhost:5258`

---

## API Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | /games | Get all games | ❌ |
| GET | /games/{id} | Get game by ID | ❌ |
| POST | /games | Create a new game | ✅ |
| PUT | /games/{id} | Update a game | ✅ |
| DELETE | /games/{id} | Delete a game | ✅ |

---

## Stopping SQL Server
```powershell
docker stop mssql
```

---

## Notes
- Database is automatically created and migrated on startup
- JWT token is required for POST, PUT and DELETE endpoints
- Use `dotnet user-jwts create` to generate a JWT token for testing