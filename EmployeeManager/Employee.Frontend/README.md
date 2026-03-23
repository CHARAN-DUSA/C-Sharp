<div align="center">

<img src="https://img.shields.io/badge/Angular-19-DD0031?style=for-the-badge&logo=angular&logoColor=white" />
<img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
<img src="https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" />
<img src="https://img.shields.io/badge/Docker-WSL2-2496ED?style=for-the-badge&logo=docker&logoColor=white" />
<img src="https://img.shields.io/badge/JWT-Auth-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white" />
<img src="https://img.shields.io/badge/Bootstrap-5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white" />

<br/><br/>

# 🏢 Employee Management System

### A production-ready full-stack web application for managing employees, departments, and designations — built with Angular 19 and .NET 9 Web API.

<br/>

[📸 Screenshots](#-screenshots) · [✨ Features](#-features) · [🚀 Quick Start](#-quick-start) · [🏗️ Architecture](#️-architecture) · [🔐 Security](#-security) · [🗄️ Database](#️-database)

</div>

---

## 📌 About The Project

**EmpAdmin** is a role-based employee management portal that allows HR staff to manage the full employee lifecycle — from onboarding to profile updates — while giving employees secure access to view and update their own profiles.

Built as a learning project following **Julio Casal's .NET course**, this system demonstrates real-world patterns including:

- ✅ Clean REST API with EF Core + SQL Server
- ✅ Angular Signals for reactive state management
- ✅ JWT authentication with HTTP interceptors
- ✅ Role-based route guards
- ✅ sessionStorage for secure client-side sessions
- ✅ Toast notifications replacing all `alert()` calls
- ✅ Skeleton loading screens for better UX
- ✅ Docker-based SQL Server on Windows (WSL2)

---

## ✨ Features

### 👥 Role-Based Access Control

| Role | Access | Permissions |
|------|--------|-------------|
| **HR** | Dashboard, Employees, Departments, Designations | Full CRUD on all entities |
| **Employee** | My Profile only | View & update own profile |

### 📊 HR Dashboard
- Live stats: total employees, HR count, department count, designation count
- Quick action buttons for common tasks
- Recent employees table

### 👤 Employee Management
- Full employee list with department, designation, and role badges
- Add / Edit form with skeleton loading on edit
- Loading button states to prevent double-submit
- Confirm before delete with per-row loading indicator

### 🏢 Department & Designation Management
- Full CRUD with inline form editing
- Reactive Forms with validation
- Designations linked to departments via foreign key

### 🙋 My Profile (Employee Role)
- Avatar with initials, role badge, contact and address cards
- Toggle edit mode to update personal details
- Profile saved with optimistic loading feedback

### 🔒 Authentication
- Email + password login with loading state
- JWT token-based API authentication
- Auto-logout on 401 (session expiry)
- sessionStorage — clears automatically on tab close

---

## 🚀 Quick Start

### Prerequisites

```bash
node --version      # 20+
dotnet --version    # 9.0+
ng version          # Angular CLI 19+
docker --version    # Any recent version
```

### 1. Clone the repository

```bash
git clone https://github.com/CHARAN-DUSA/C-Sharp.git
cd C-Sharp
```

### 2. Start SQL Server (Docker via WSL2 on Windows)

```bash
# In WSL terminal
sudo service docker start
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password@123" \
  -p 1433:1433 --name sqlserver \
  -v sqlvolume:/var/opt/mssql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

```powershell
# In Admin PowerShell (Windows) — bridge WSL2 network
netsh interface portproxy add v4tov4 `
  listenport=1433 listenaddress=0.0.0.0 `
  connectport=1433 connectaddress=$(wsl hostname -I)
```

### 3. Set up and run the Backend

```bash
cd Employee.Api

# Set connection string via user secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=localhost,1433;Database=EmployeeDB;User Id=sa;Password=Password@123;TrustServerCertificate=True"

# Apply EF Core migrations (auto-creates database)
dotnet ef database update

# Run the API
dotnet run
```

> API available at: `https://localhost:7033`
> Swagger UI at: `https://localhost:7033/swagger`

### 4. Set up and run the Frontend

```bash
cd Employee.Frontend

npm install

ng serve
```

> App available at: `http://localhost:4200`

---

## 🔄 Daily Development Workflow

Every time you return to development, follow this order:

```
1. WSL terminal        →  sudo service docker start
2. WSL terminal        →  docker start sqlserver
3. Admin PowerShell    →  run netsh port proxy command
4. PowerShell          →  cd Employee.Api  →  dotnet run
5. PowerShell          →  cd Employee.Frontend  →  ng serve
```

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────┐
│                     Browser (Angular 19)                 │
│  ┌──────────┐  ┌──────────┐  ┌───────────┐  ┌────────┐ │
│  │ Dashboard│  │Employees │  │Departments│  │Profile │ │
│  └──────────┘  └──────────┘  └───────────┘  └────────┘ │
│       ↓              ↓              ↓             ↓      │
│  ┌─────────────────────────────────────────────────────┐ │
│  │  Services (HttpClient + Interceptors + Guards)      │ │
│  └─────────────────────────────────────────────────────┘ │
└───────────────────────┬─────────────────────────────────┘
                        │ HTTPS + JWT Bearer
┌───────────────────────▼─────────────────────────────────┐
│              .NET 9 Web API (ASP.NET Core)               │
│  ┌──────────────────┐   ┌──────────────────────────────┐ │
│  │   Controllers    │   │  Middleware Pipeline         │ │
│  │  - Employee      │   │  - JWT Auth                  │ │
│  │  - Department    │   │  - CORS                      │ │
│  │  - Designation   │   │  - HTTPS Redirect            │ │
│  └──────────────────┘   └──────────────────────────────┘ │
│  ┌─────────────────────────────────────────────────────┐ │
│  │         Entity Framework Core (Code First)          │ │
│  └─────────────────────────────────────────────────────┘ │
└───────────────────────┬─────────────────────────────────┘
                        │
┌───────────────────────▼─────────────────────────────────┐
│          SQL Server 2022 (Docker Container)              │
│   DepartmentMaster → DesignationMaster → EmployeeMaster  │
└─────────────────────────────────────────────────────────┘
```

### Frontend Structure

```
src/
├── app/
│   ├── components/
│   │   └── toast/                  # Global toast notification component
│   ├── guards/
│   │   └── auth.guard.ts           # authGuard + hrGuard
│   ├── interceptors/
│   │   └── auth.interceptor.ts     # 401 auto-logout handler
│   ├── modes/                      # TypeScript models & interfaces
│   ├── pages/
│   │   ├── login/                  # Login page
│   │   ├── header/                 # Sidebar layout + router-outlet
│   │   ├── dashboard/              # HR stats & quick actions
│   │   ├── employee-form/          # Add / Edit employee
│   │   ├── employee-list/          # Employee table
│   │   ├── department/             # Department CRUD
│   │   ├── designation/            # Designation CRUD
│   │   └── my-profile/             # Employee self-profile
│   └── services/
│       ├── employee-service.ts     # Employee API methods
│       ├── master.ts               # Dept & Designation API methods
│       ├── session.service.ts      # sessionStorage wrapper
│       └── toast.service.ts        # Signal-based toast service
└── environments/
    ├── environment.ts              # Dev API URL
    └── environment.prod.ts         # Production API URL
```

---

## 🔐 Security

### JWT Authentication
- Backend issues a signed JWT on successful login
- Angular `authInterceptor` catches **401 Unauthorized** responses and auto-clears the session + redirects to login
- Token should be set to expire in 60 minutes in production

### sessionStorage vs localStorage

| | localStorage | sessionStorage ✅ used here |
|--|--|--|
| **Cleared on tab close** | ❌ No | ✅ Yes |
| **Cross-tab access** | ❌ Yes (risky) | ✅ No (isolated) |
| **Risk on shared PC** | ❌ High | ✅ Low |

### Route Guards

```typescript
// authGuard — blocks unauthenticated access
// hrGuard  — blocks Employee role from HR pages
```

All HR routes are protected by `hrGuard`. Even if an employee manually types `/dashboard` in the browser, they are immediately redirected to `/my-profile`.

### CORS

```csharp
// Program.cs — allow Angular dev server
policy.WithOrigins("http://localhost:4200")
      .AllowAnyHeader()
      .AllowAnyMethod();
```

> ⚠️ Update this to your production domain before deploying.

### Password Security

> ⚠️ The current implementation uses contact number as a password for demo purposes. Before going to production, implement proper password hashing:

```csharp
// Install: BCrypt.Net-Next
string hash   = BCrypt.Net.BCrypt.HashPassword(plainTextPassword);
bool   valid  = BCrypt.Net.BCrypt.Verify(plainTextPassword, hash);
```

---

## 🗄️ Database

### Schema

```
DepartmentMaster
  └── DepartmentId (PK)
  └── DepartmentName
  └── IsActive
       │
       ▼
DesignationMaster
  └── DesignationId (PK)
  └── DesignationName
  └── DepartmentId (FK → DepartmentMaster)
       │
       ▼
EmployeeMaster
  └── EmployeeId (PK)
  └── Name, Email, ContactNo, AltContactNo
  └── City, State, Pincode, Address
  └── Role (HR | Employee)
  └── DesignationId (FK → DesignationMaster)
  └── CreatedDate, ModifiedDate
```

### EF Core Migrations

```bash
# Create a new migration after model changes
dotnet ef migrations add YourMigrationName

# Apply to database
dotnet ef database update
```

> The API calls `Database.Migrate()` on startup — schema auto-updates every time the API runs.

### Connection String (User Secrets — never commit to Git)

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=localhost,1433;Database=EmployeeDB;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True"
```

---

## 🛠️ Tech Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Frontend Framework | Angular (Standalone + Signals) | 19 |
| UI | Bootstrap + Font Awesome | 5 |
| Backend Framework | ASP.NET Core Web API | .NET 9 |
| ORM | Entity Framework Core | 9 |
| Database | SQL Server | 2022 |
| Container | Docker (WSL2) | Latest |
| Auth | JWT Bearer Tokens | — |
| Session | Browser sessionStorage | — |

---

## 🚧 Roadmap

- [ ] Password hashing with BCrypt
- [ ] Pagination on employee list
- [ ] Search and filter on employee list
- [ ] Leave management module
- [ ] Email notifications on employee creation
- [ ] Export employee list to Excel / PDF
- [ ] Dark mode toggle
- [ ] Unit tests with xUnit (.NET) and Jasmine (Angular)
- [ ] CI/CD pipeline with GitHub Actions
- [ ] Deploy to Azure (App Service + Azure SQL)

---

## 🐛 Troubleshooting

<details>
<summary><b>API returns 307 Redirect</b></summary>

Your `apiUrl` uses `http://` but the API requires `https://`. Update `src/environments/environment.ts`:
```typescript
apiUrl: 'https://localhost:7033/api'
```
</details>

<details>
<summary><b>Skeleton shows forever / data never loads</b></summary>

Check the browser Console (F12) for errors. Make sure `isLoading.set(false)` is called in **both** `next` and `error` callbacks of your subscribe.
</details>

<details>
<summary><b>400 Bad Request on Save</b></summary>

Open DevTools → Network → click the failed request → Response tab. Usually caused by `designationId` being sent as a string `"0"` instead of a number. Use `[ngValue]` instead of `[value]` on the `<option>` element in the designation dropdown.
</details>

<details>
<summary><b>Docker / SQL Server not connecting</b></summary>

```bash
# Check if container is running
docker ps

# Start it if stopped
docker start sqlserver

# Re-run port proxy in Admin PowerShell
netsh interface portproxy add v4tov4 listenport=1433 listenaddress=0.0.0.0 connectport=1433 connectaddress=$(wsl hostname -I)
```
</details>

<details>
<summary><b>Cannot find module environments/environment</b></summary>

Make sure the file is at `src/environments/environment.ts` — **not** inside `src/app/environments/`.
</details>

<details>
<summary><b>Angular signal warning: isLoading is a function</b></summary>

Signals must be called as functions in templates. Change `isLoading` to `isLoading()` everywhere in HTML.
</details>

---

## 📋 Production Checklist

- [ ] Change SA password to a strong unique value
- [ ] Move connection string to environment variables or Azure Key Vault
- [ ] Set JWT secret to a 256-bit random key
- [ ] Set JWT expiry to 60 minutes
- [ ] Implement BCrypt password hashing
- [ ] Update CORS to production domain only
- [ ] Remove or protect Swagger UI
- [ ] Update `environment.prod.ts` with production API URL
- [ ] Run `ng build --configuration production`
- [ ] Use managed SQL (Azure SQL / AWS RDS) instead of Docker
- [ ] Enable automated database backups

---

## 👨‍💻 Author

**Charan Dusa**

[![GitHub](https://img.shields.io/badge/GitHub-CHARAN--DUSA-181717?style=for-the-badge&logo=github)](https://github.com/CHARAN-DUSA)

---

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

---

<div align="center">

**⭐ If this project helped you, please give it a star — it really helps!**

Built with ❤️ using Angular 19 + .NET 9 + SQL Server

</div>