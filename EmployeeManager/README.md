<div align="center">

<img src="https://img.shields.io/badge/Angular-19-DD0031?style=for-the-badge&logo=angular&logoColor=white" />
<img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
<img src="https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" />
<img src="https://img.shields.io/badge/Docker-WSL2-2496ED?style=for-the-badge&logo=docker&logoColor=white" />
<img src="https://img.shields.io/badge/JWT-Auth-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white" />
<img src="https://img.shields.io/badge/Bootstrap-5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white" />
<img src="https://img.shields.io/badge/Deployed%20on-Vercel-000000?style=for-the-badge&logo=vercel&logoColor=white" />
<img src="https://img.shields.io/badge/Backend%20on-Render-46E3B7?style=for-the-badge&logo=render&logoColor=white" />
<img src="https://img.shields.io/badge/Database-Azure%20SQL-0078D4?style=for-the-badge&logo=microsoftazure&logoColor=white" />

<br/><br/>

# 🏢 Employee Management System

### A production-ready full-stack web application for managing employees, salary, tasks, leaves, attendance, and announcements — built with Angular 19 and .NET 9 Web API.

<br/>

[✨ Features](#-features) · [🚀 Quick Start](#-quick-start) · [☁️ Deployment](#️-deployment) · [🏗️ Architecture](#️-architecture) · [📄 Pages](#-pages-overview) · [🔐 Security](#-security) · [🗄️ Database](#️-database) · [🐛 Troubleshooting](#-troubleshooting)

<br/>


</div>

---

## 📌 About The Project

**Workforce** is a role-based employee management portal that handles the complete employee lifecycle — from onboarding and salary management to leave approvals, daily attendance tracking, task assignments, and company-wide announcements.

Built as a hands-on learning project following **Julio Casal's .NET course**, this system demonstrates real-world full-stack patterns:

- ✅ Clean REST API with EF Core + SQL Server
- ✅ Angular Signals for reactive state management
- ✅ JWT authentication with HTTP interceptors
- ✅ Role-based route guards (HR vs Employee)
- ✅ sessionStorage for secure client-side sessions
- ✅ Toast notifications replacing all `alert()` calls
- ✅ Skeleton loading screens for better UX
- ✅ Docker-based SQL Server for local development
- ✅ Azure SQL Free Tier for cloud database
- ✅ Check-in / Check-out attendance system
- ✅ Printable salary slips
- ✅ Leave approval workflow with rejection reasons
- ✅ Task assignment with priority and overdue detection

---

## ✨ Features

### 👥 Role-Based Access Control

| Role | Pages | Permissions |
|------|-------|-------------|
| **HR** | Dashboard, Employees, Salary, Tasks, Leaves, Attendance, Announcements, Departments, Designations | Full CRUD on all entities |
| **Employee** | My Profile, Attendance, My Salary Slips, My Leaves, My Tasks, Announcements | View and manage own data only |

---

### 📊 HR Dashboard
- Live stats: total employees, HR count, department count, designation count, overdue tasks
- Quick action buttons for common HR tasks
- Recent employees table with department and role badges

---

### 👤 Employee Management
- Full employee list with department, designation, and role badges
- Add / Edit form with sections: Basic Info, Contact, Address, Job Info
- Skeleton loading on edit form while data fetches
- Loading button states to prevent double-submit
- Confirm before delete with per-row loading indicator

---

### 💰 Salary Management + Salary Slips

**HR side — `/salary`**
- Add monthly salary records (Basic, HRA, DA, Bonus, Deductions)
- Net salary computed automatically with live preview
- Edit and delete existing records
- Duplicate month/year prevention per employee

**Employee side — `/my-salary`**
- View all own salary slips as clickable cards
- Formatted printable salary slip modal
- Full breakdown: Basic → HRA → DA → Bonus → Deductions → **Net Salary**
- Print / Download as PDF via browser print dialog

---

### ✅ Task Management
- HR assigns tasks with title, description, due date, priority (Low / Medium / High)
- Status: **Pending → In Progress → Completed**
- Overdue detection with red highlight
- Employees update status with optional completion note
- Filter by status using clickable stat pills

---

### 📅 Leave Management
- Employee applies for Casual / Sick / Earned / Unpaid leave
- Employee can cancel pending requests
- HR approves or rejects with a typed reason
- Rejection reason shown to employee
- Total days auto-calculated on backend

---

### 🕐 Attendance

**Employee:** One-click Check In / Check Out · Working hours calculated · Half Day if < 4h · Monthly summary

**HR:** Monitor all attendance · Add manual entries · Filter by month/year

---

### 📢 Announcements
- HR posts with target audience: Everyone / HR Only / Employees Only
- Employees see only relevant announcements
- Soft delete on HR side

---

### 🏢 Departments & Designations
- Full CRUD with inline editing and validation
- Designations linked to departments via FK

---

### 🙋 My Profile
- Auto-generated initials avatar
- Toggle edit mode for own details

---

## 🚀 Quick Start

### Prerequisites

```
Node.js 20+   |   .NET 9 SDK   |   Angular CLI 19+   |   Docker (for local DB)
```

### 1. Clone

```bash
git clone https://github.com/CHARAN-DUSA/C-Sharp.git
cd EmployeeManager
```

---

### Option A — Local Setup (Docker + SSMS)

#### Start SQL Server

```bash
# WSL terminal
sudo service docker start

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStr0ngPassword!" \
  -p 1433:1433 --name sqlserver \
  -v sqlvolume:/var/opt/mssql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

```powershell
# Admin PowerShell — bridge WSL2 to Windows
netsh interface portproxy add v4tov4 `
  listenport=1433 listenaddress=0.0.0.0 `
  connectport=1433 connectaddress=$(wsl hostname -I)
```

#### Connect via SSMS

| Field | Value |
|-------|-------|
| Server name | `localhost,1433` |
| Authentication | SQL Server Authentication |
| Login | `sa` |
| Password | *(your chosen password)* |

#### Run the Backend

```bash
cd Employee.Api

dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design

dotnet user-secrets set "ConnectionStrings:empCon" \
  "Server=localhost,1433;Database=employeeManageDb;User Id=sa;Password=YourStr0ngPassword!;TrustServerCertificate=True"

dotnet ef migrations add InitialSetup
dotnet ef database update

dotnet run
```

> API: `https://localhost:7033` · Swagger: `https://localhost:7033/swagger`

---

### Option B — Cloud Setup (Azure SQL Free Tier)

1. Go to [portal.azure.com](https://portal.azure.com)
2. Create **SQL Database** → select **Free Tier** (32 GB, no cost)
3. Under **Networking** → add your IP to firewall rules
4. Set connection string:

```bash
dotnet user-secrets set "ConnectionStrings:empCon" \
  "Server=your-server.database.windows.net;Database=your-db;User Id=your-user;Password=xxxxx;TrustServerCertificate=True"
```

5. Apply migrations:

```bash
dotnet ef database update
```

---

### Frontend

```bash
cd Employee.Frontend
npm install
ng serve          # development → http://localhost:4200
ng build --configuration production   # production build
```

---

## 🔄 Daily Local Workflow

```
1. WSL          →  sudo service docker start
2. WSL          →  docker start sqlserver
3. Admin PS     →  netsh port proxy command
4. PowerShell   →  dotnet run  (in Employee.Api)
5. PowerShell   →  ng serve    (in Employee.Frontend)
```

---

## ☁️ Deployment

| Layer | Platform | Free Tier |
|-------|----------|-----------|
| Frontend | **Vercel** | Unlimited static deploys |
| Backend API | **Render** | 750 hrs/month (spins down after 15 min idle) |
| Database | **Azure SQL** | 32 GB, always-on |

---

### Frontend → Vercel

1. Push to GitHub
2. [vercel.com](https://vercel.com) → **Add New Project** → select repo
3. Framework: `Angular`
4. Build Command: `ng build --configuration production`
5. Output Directory: `dist/your-app-name/browser`
6. Add environment variable:
   ```
   API_URL = https://your-render-app.onrender.com/api
   ```
7. Deploy — every push to `main` auto-redeploys

---

### Backend → Render

1. [render.com](https://render.com) → **New Web Service** → connect repo
2. Runtime: `.NET`
3. Build Command: `dotnet publish -c Release -o out`
4. Start Command: `dotnet out/Employee.Api.dll`
5. Environment Variables:
   ```
   ConnectionStrings__empCon = Server=xxxxx.database.windows.net;Database=xxxxx;User Id=xxxxx;Password=xxxxx;TrustServerCertificate=True
   ```

> ⚠️ Free tier spins down after 15 min idle. First request after sleep takes ~30–60 seconds.

---

### Database → Azure SQL Free Tier

1. Create SQL Database at [portal.azure.com](https://portal.azure.com)
2. Choose **Free offer** (32 GB)
3. Networking → add Render outbound IPs to firewall
4. Copy ADO.NET connection string from portal
5. Set as environment variable in Render (never commit it)

---

### Update before deploying frontend

```typescript
// src/environments/environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://your-render-app.onrender.com/api'
};
```

---

## 🏗️ Architecture

```
┌──────────────────────────────────────────────────┐
│               Browser — Angular 19                │
│                                                    │
│  HR:   Dashboard · Employees · Salary · Tasks     │
│        Leaves · Attendance · Announcements        │
│        Departments · Designations                  │
│                                                    │
│  EMP:  Profile · Attendance · Salary Slips        │
│        Leaves · Tasks · My Announcements           │
│                                                    │
│  Shared: Services · Interceptor · Guards · Toast  │
└──────────────────┬───────────────────────────────┘
                   │  HTTPS + JWT
         ┌─────────▼──────────┐
         │  Render (.NET 9)   │
         │  8 Controllers     │
         │  EF Core           │
         └─────────┬──────────┘
                   │
         ┌─────────▼──────────┐
         │  Azure SQL         │
         │  8 Tables          │
         └────────────────────┘
```

### Frontend Folder Structure

```
src/
├── app/
│   ├── auth/
│   │   └── login/
│   │
│   ├── common/                   # Both HR and Employee
│   │   ├── attendance/
│   │   ├── leaves/
│   │   └── tasks/
│   │
│   ├── employee/                 # Employee-only
│   │   ├── dashboard/
│   │   ├── my-announcements/
│   │   ├── my-profile/
│   │   └── salary-slips/
│   │
│   ├── hrAdmin/                  # HR-only
│   │   ├── announcements/
│   │   ├── department/
│   │   ├── designation/
│   │   ├── employee-form/
│   │   ├── employee-list/
│   │   └── salary/
│   │
│   ├── layout/
│   │   └── header/               # Sidebar + router-outlet
│   │
│   └── shared/
│       ├── components/toast/
│       ├── guards/auth.guard.ts
│       ├── interceptors/
│       ├── models/
│       │   ├── Employee.model.ts
│       │   └── Department.models.ts
│       └── services/
│           ├── employee-service.ts
│           ├── master.ts
│           ├── session.service.ts
│           └── toast.service.ts
│
└── environments/
    ├── environment.ts            # Dev URL
    └── environment.prod.ts       # Render URL
```

---

## 🗄️ Database

### Tables

| Table | Purpose |
|-------|---------|
| `departmentTbl` | Departments |
| `designationTbl` | Job titles linked to departments |
| `employeeTbl` | Employee personal and job data |
| `salaryTbl` | Monthly salary records |
| `taskTbl` | Assigned tasks |
| `leaveTbl` | Leave applications |
| `attendanceTbl` | Daily check-in/out (unique per employee per day) |
| `announcementTbl` | Announcements (soft delete) |

### EF Core Migrations

```bash
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

---

## 🔐 Security

### sessionStorage vs localStorage

| | localStorage | sessionStorage ✅ |
|--|--|--|
| Cleared on tab close | ❌ No | ✅ Yes |
| Cross-tab sharing | ❌ Yes | ✅ No |
| Risk on shared PC | ❌ High | ✅ Low |

### Route Guards
```typescript
authGuard  →  unauthenticated users to /login
hrGuard    →  Employee role away from HR pages
```

### CORS
```csharp
// Update to your Vercel domain before deploying
policy.WithOrigins("https://your-app.vercel.app")
      .AllowAnyHeader().AllowAnyMethod();
```

### Never Commit Secrets
```bash
# Local — use .NET User Secrets
dotnet user-secrets set "ConnectionStrings:empCon" "xxxxx"

# Production — use Render Environment Variables
```

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Angular 19 (Standalone + Signals) |
| UI | Bootstrap 5 + Font Awesome |
| Backend | ASP.NET Core Web API (.NET 9) |
| ORM | Entity Framework Core 9 |
| Local DB | SQL Server 2022 via Docker |
| Cloud DB | Azure SQL Free Tier |
| Frontend Host | Vercel |
| Backend Host | Render |
| Auth | JWT Bearer Tokens |
| Session | Browser sessionStorage |

---

## 🚧 Roadmap

- [ ] BCrypt password hashing
- [ ] Leave balance tracking per year
- [ ] Pagination on employee list
- [ ] Excel export for attendance
- [ ] PDF export for salary slips
- [ ] Email notifications for leave actions
- [ ] Dark mode
- [ ] Unit tests — xUnit + Jasmine
- [ ] GitHub Actions CI/CD

---

## 🐛 Troubleshooting

<details>
<summary><b>API returns 307 Redirect</b></summary>

Change `http://` to `https://` in `environment.ts`:
```typescript
apiUrl: 'https://localhost:7033/api'
```
</details>

<details>
<summary><b>EF migration fails — table already exists</b></summary>

Create the history table in SSMS first:
```sql
USE employeeManageDb;
CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId] NVARCHAR(150) NOT NULL,
    [ProductVersion] NVARCHAR(32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);
```
Then empty the `Up()` method of your migration and run `dotnet ef database update`.
</details>

<details>
<summary><b>Decimal warnings on SalaryModel</b></summary>

Add `[Column(TypeName = "decimal(18,2)")]` to all decimal properties.
</details>

<details>
<summary><b>Skeleton never disappears</b></summary>

Ensure `isLoading.set(false)` is in **both** `next` and `error` callbacks.
</details>

<details>
<summary><b>Check-in says "Already checked in"</b></summary>

One entry per employee per day is enforced. Delete today's row in SSMS to reset for testing.
</details>

<details>
<summary><b>Render backend slow on first request</b></summary>

Free tier spins down after 15 min idle. First request takes 30–60s to wake up. Expected behaviour.
</details>

<details>
<summary><b>CORS error after Render deploy</b></summary>

Update `Program.cs` CORS origin to your Vercel URL and redeploy.
</details>

<details>
<summary><b>Angular signal warning in template</b></summary>

Use `isLoading()` not `isLoading` — signals must be called as functions in HTML.
</details>

---

## 📋 Production Checklist

- [ ] All passwords set to strong unique values (no `xxxxx` placeholders)
- [ ] Connection string in Render environment variables only
- [ ] JWT secret key is 256-bit random with 60-min expiry
- [ ] BCrypt password hashing implemented
- [ ] CORS updated to Vercel domain
- [ ] Swagger UI removed or protected
- [ ] `environment.prod.ts` points to Render URL
- [ ] `ng build --configuration production` run before Vercel deploy
- [ ] Azure SQL firewall includes Render outbound IPs
- [ ] Automated backups enabled on Azure SQL

---

## 👨‍💻 Author

**GitHub:** [@CHARAN-DUSA](https://github.com/CHARAN-DUSA)


<div align="center">

**⭐ If this project helped you, please give it a star!**

Built with ❤️ using Angular 19 + .NET 9 + Azure SQL · Hosted on Vercel + Render

</div>