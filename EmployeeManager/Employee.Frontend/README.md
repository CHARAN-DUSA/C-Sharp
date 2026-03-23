<div align="center">

<img src="https://img.shields.io/badge/Angular-19-DD0031?style=for-the-badge&logo=angular&logoColor=white" />
<img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
<img src="https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" />
<img src="https://img.shields.io/badge/Docker-WSL2-2496ED?style=for-the-badge&logo=docker&logoColor=white" />
<img src="https://img.shields.io/badge/JWT-Auth-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white" />
<img src="https://img.shields.io/badge/Bootstrap-5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white" />
<img src="https://img.shields.io/badge/EF%20Core-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />

<br/><br/>

# 🏢 Employee Management System

### A production-ready full-stack web application for managing employees, salary, tasks, leaves, attendance, and announcements — built with Angular 19 and .NET 9 Web API.

<br/>

[✨ Features](#-features) · [🚀 Quick Start](#-quick-start) · [🏗️ Architecture](#️-architecture) · [📄 Pages](#-pages-overview) · [🔐 Security](#-security) · [🗄️ Database](#️-database) · [🐛 Troubleshooting](#-troubleshooting)

</div>

---

## 📌 About The Project

**EmpAdmin** is a role-based employee management portal that handles the complete employee lifecycle — from onboarding and salary management to leave approvals, daily attendance tracking, task assignments, and company-wide announcements.

Built as a hands-on learning project following **Julio Casal's .NET course**, this system demonstrates real-world full-stack patterns:

- ✅ Clean REST API with EF Core + SQL Server
- ✅ Angular Signals for reactive state management
- ✅ JWT authentication with HTTP interceptors
- ✅ Role-based route guards (HR vs Employee)
- ✅ sessionStorage for secure client-side sessions
- ✅ Toast notifications replacing all `alert()` calls
- ✅ Skeleton loading screens for better UX
- ✅ Docker-based SQL Server on Windows (WSL2)
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
- Quick action buttons for the most common HR tasks
- Recent employees table with department and role badges

---

### 👤 Employee Management
- Full employee list with department, designation, and role badges
- Add / Edit form split into sections: Basic Info, Contact, Address, Job Info
- Skeleton loading on edit form while data fetches from API
- Loading button states to prevent double-submit
- Confirm before delete with per-row loading indicator

---

### 💰 Salary Management + Salary Slips

**HR side — `/salary`**
- Add monthly salary records per employee (Basic, HRA, DA, Bonus, Deductions)
- Net salary computed automatically with a live preview before saving
- Edit and delete existing salary records
- Duplicate month/year prevention per employee enforced on backend

**Employee side — `/my-salary`**
- View all own salary slips as clickable cards
- Click any month to open a formatted, printable salary slip modal
- Full breakdown: Basic → HRA → DA → Bonus → Deductions → **Net Salary**
- Print button triggers browser print dialog (works as PDF download too)

> HR manages the data. Employees see their own slice as formatted slips. Same database, different perspective.

---

### ✅ Task Management
- HR assigns tasks with title, description, due date, and priority (Low / Medium / High)
- Status tracking: **Pending → In Progress → Completed**
- Overdue task detection with red card highlight
- Employees update their own task status with an optional completion note
- Clickable stat pills to filter tasks by status
- Card-based layout for easy at-a-glance scanning

---

### 📅 Leave Management
- **Employee:** Apply for Casual, Sick, Earned, or Unpaid leave with date range and reason
- **Employee:** Cancel own pending leave requests before HR acts on them
- **HR:** View all leave requests, filter pending ones
- **HR:** Approve with one click or reject with a typed rejection reason
- Rejection reason is shown to the employee in the table
- Total days auto-calculated from date range on the backend

| Status | Color |
|--------|-------|
| Pending | 🟡 Yellow |
| Approved | 🟢 Green |
| Rejected | 🔴 Red |

---

### 🕐 Attendance

**Employee side:**
- One-click **Check In** — records current timestamp automatically
- **Check Out** appears after check-in — calculates working hours
- Working hours < 4 automatically marks as **Half Day**
- Today's status panel: shows check-in time, check-out time, and hours worked
- Monthly summary cards: Present / Absent / Half Day / On Leave / Total Hours
- Filter attendance history by month and year

**HR side:**
- Monitor all employee attendance in a sortable table
- Filter by month and year
- Add manual attendance entries for corrections
- Status options: Present / Absent / Half Day / Leave
- Unique constraint prevents duplicate entries per employee per day

---

### 📢 Announcements

**HR side — `/announcements`**
- Post announcements with title, content, and target audience
- Target options: **Everyone / HR Only / Employees Only**
- Edit and soft-delete announcements

**Employee side — `/my-announcements`**
- Automatically filtered to show only relevant announcements (All + own role)
- Color-coded dots by audience type
- Clean chronological list view

---

### 🏢 Department & Designation Management
- Full CRUD with inline form editing at top of page
- Reactive Forms with required field validation
- Unique name enforcement on backend
- Designations linked to departments via foreign key dropdown

---

### 🙋 My Profile (Employee)
- Avatar with auto-generated initials from employee name
- Role badge, designation, contact info cards, address cards
- Toggle edit mode to update own personal details
- Saving state shown on update button

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

### 2. Start SQL Server via Docker (WSL2 on Windows)

```bash
# In WSL terminal
sudo service docker start

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password@123" \
  -p 1433:1433 --name sqlserver \
  -v sqlvolume:/var/opt/mssql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

```powershell
# In Admin PowerShell — bridge WSL2 network to Windows
netsh interface portproxy add v4tov4 `
  listenport=1433 listenaddress=0.0.0.0 `
  connectport=1433 connectaddress=$(wsl hostname -I)
```

### 3. Set up and run the Backend

```bash
cd Employee.Api

# Install required EF Core packages
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design

# Set connection string securely via user secrets
dotnet user-secrets set "ConnectionStrings:empCon" \
  "Server=localhost,1433;Database=employeeManageDb;User Id=sa;Password=Password@123;TrustServerCertificate=True"

# Create and apply database migrations
dotnet ef migrations add InitialSetup
dotnet ef database update

# Start the API
dotnet run
```

> API runs at: `https://localhost:7033`  
> Swagger UI at: `https://localhost:7033/swagger`

### 4. Set up and run the Frontend

```bash
cd Employee.Frontend

npm install

ng serve
```

> App runs at: `http://localhost:4200`

---

## 🔄 Daily Development Workflow

Follow this order every time you start development:

```
1. WSL terminal        →  sudo service docker start
2. WSL terminal        →  docker start sqlserver
3. Admin PowerShell    →  run netsh port proxy command
4. PowerShell          →  cd Employee.Api  →  dotnet run
5. PowerShell          →  cd Employee.Frontend  →  ng serve
```

---

## 📄 Pages Overview

### HR Pages

| Route | Page | Description |
|-------|------|-------------|
| `/dashboard` | Dashboard | Stats cards, quick actions, recent employees |
| `/employees` | Employee List | Full CRUD employee table |
| `/new-employee` | Add Employee | Multi-section form |
| `/new-employee/:id` | Edit Employee | Pre-filled with skeleton loading |
| `/salary` | Salary Management | Add/edit/delete monthly salary records |
| `/tasks` | Task Management | Assign and monitor all employee tasks |
| `/leaves` | Leave Requests | Approve or reject with reason modal |
| `/attendance` | Attendance Monitor | View all + add manual entries |
| `/announcements` | Announcements | Post with audience targeting |
| `/departments` | Departments | CRUD with active/inactive status |
| `/designations` | Designations | CRUD linked to departments |

### Employee Pages

| Route | Page | Description |
|-------|------|-------------|
| `/my-profile` | My Profile | View and edit own personal details |
| `/attendance` | My Attendance | Check in/out + monthly summary cards |
| `/my-salary` | Salary Slips | View and print own formatted salary slips |
| `/leaves` | My Leaves | Apply, track, and cancel leave requests |
| `/tasks` | My Tasks | View assigned tasks, update status with notes |
| `/my-announcements` | Announcements | Role-filtered company announcements |

---

## 🏗️ Architecture

```
┌──────────────────────────────────────────────────────────────────┐
│                       Browser (Angular 19)                        │
│                                                                    │
│  HR:  Dashboard │ Employees │ Salary │ Tasks │ Leaves             │
│       Attendance │ Announcements │ Departments │ Designations      │
│                                                                    │
│  EMP: Profile │ Attendance │ Salary Slips │ Leaves │ Tasks        │
│       My Announcements                                             │
│                                                                    │
│  ┌──────────────────────────────────────────────────────────┐     │
│  │  Services │ HTTP Interceptor │ Route Guards │ Signals     │     │
│  └──────────────────────────────────────────────────────────┘     │
└───────────────────────────┬──────────────────────────────────────┘
                            │ HTTPS + JWT Bearer Token
┌───────────────────────────▼──────────────────────────────────────┐
│                    .NET 9 Web API (ASP.NET Core)                   │
│                                                                    │
│  EmployeeMaster │ Department │ Designation │ Salary │ Task        │
│  Leave │ Attendance │ Announcement                                 │
│                                                                    │
│  ┌──────────────────────────────────────────────────────────┐     │
│  │              Entity Framework Core (Code First)           │     │
│  └──────────────────────────────────────────────────────────┘     │
└───────────────────────────┬──────────────────────────────────────┘
                            │
┌───────────────────────────▼──────────────────────────────────────┐
│                 SQL Server 2022 (Docker Container)                 │
│                                                                    │
│  departmentTbl ──► designationTbl ──► employeeTbl                 │
│                                            │                       │
│                    ┌───────────────────────┼──────────────┐       │
│                    ▼                       ▼              ▼        │
│                salaryTbl              taskTbl         leaveTbl     │
│                                                                    │
│                attendanceTbl          announcementTbl              │
└────────────────────────────────────────────────────────────────────┘
```

### Frontend Folder Structure

```
src/
├── app/
│   ├── components/toast/             # Global toast notifications
│   ├── guards/auth.guard.ts          # authGuard + hrGuard
│   ├── interceptors/                 # 401 auto-logout interceptor
│   ├── modes/                        # TypeScript models & interfaces
│   ├── pages/
│   │   ├── login/
│   │   ├── header/                   # Sidebar + router-outlet
│   │   ├── dashboard/                # HR dashboard
│   │   ├── employee-form/            # Add / Edit employee
│   │   ├── employee-list/            # Employee table
│   │   ├── salary/                   # HR: manage salary records
│   │   ├── salary-slip/              # Employee: view & print slips
│   │   ├── tasks/                    # Both roles: task management
│   │   ├── leaves/                   # Both roles: leave management
│   │   ├── attendance/               # Both roles: attendance
│   │   ├── announcements/            # HR: manage announcements
│   │   ├── my-announcements/         # Employee: view announcements
│   │   ├── department/
│   │   ├── designation/
│   │   └── my-profile/
│   └── services/
│       ├── employee-service.ts       # All API methods
│       ├── master.ts                 # Dept & Designation API
│       ├── session.service.ts        # sessionStorage wrapper
│       └── toast.service.ts          # Signal-based toast service
└── environments/
    ├── environment.ts                # Dev API URL
    └── environment.prod.ts           # Production API URL
```

---

## 🗄️ Database

### Tables

| Table | Purpose | Key Columns |
|-------|---------|-------------|
| `departmentTbl` | Departments | DepartmentId, DepartmentName, IsActive |
| `designationTbl` | Job titles | DesignationId, DesignationName, DepartmentId (FK) |
| `employeeTbl` | All employee data | EmployeeId, Name, Email, Role, DesignationId (FK) |
| `salaryTbl` | Monthly salary records | SalaryId, EmployeeId (FK), BasicSalary, HRA, DA, Bonus, Deductions, NetSalary, Month, Year |
| `taskTbl` | Assigned tasks | TaskId, Title, AssignedToEmployeeId (FK), DueDate, Status, Priority |
| `leaveTbl` | Leave requests | LeaveId, EmployeeId (FK), LeaveType, FromDate, ToDate, Status |
| `attendanceTbl` | Daily attendance | AttendanceId, EmployeeId (FK), Date, CheckIn, CheckOut, WorkingHours, Status |
| `announcementTbl` | Announcements | AnnouncementId, Title, Content, TargetRole, IsActive |

### Key Constraints
- `attendanceTbl` — unique index on `(EmployeeId, Date)` — one record per employee per day
- `salaryTbl` — unique `(EmployeeId, Month, Year)` enforced in controller
- `leaveTbl` — cancellation only allowed when `Status = 'Pending'`
- `announcementTbl` — soft delete via `IsActive = false`

### EF Core Migrations

```bash
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

---

## 🔐 Security

### JWT Authentication
- Backend issues a signed JWT on successful login
- `authInterceptor` catches **401 Unauthorized** and auto-clears session + redirects to `/login`
- Set token expiry to 60 minutes in production

### sessionStorage vs localStorage

| | localStorage | sessionStorage ✅ used here |
|--|--|--|
| Cleared on tab close | ❌ No | ✅ Yes |
| Cross-tab access | ❌ Yes (risky) | ✅ No (isolated) |
| Risk on shared PC | ❌ High | ✅ Low |

### Route Guards
```typescript
authGuard  // unauthenticated users → /login
hrGuard    // Employee role on HR routes → /my-profile
```

### CORS
```csharp
policy.WithOrigins("http://localhost:4200")
      .AllowAnyHeader().AllowAnyMethod();
```

> ⚠️ Change to your production domain before deploying.

### Password Security
> ⚠️ Current build uses contact number as password for demo. For production:

```csharp
// Install: dotnet add package BCrypt.Net-Next
string hash  = BCrypt.Net.BCrypt.HashPassword(plainText);
bool   valid = BCrypt.Net.BCrypt.Verify(plainText, hash);
```

---

## 🛠️ Tech Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Frontend | Angular Standalone + Signals | 19 |
| UI | Bootstrap + Font Awesome | 5 |
| Backend | ASP.NET Core Web API | .NET 9 |
| ORM | Entity Framework Core | 9 |
| Database | SQL Server | 2022 |
| Container | Docker via WSL2 | Latest |
| Auth | JWT Bearer Tokens | — |
| Session | Browser sessionStorage | — |

---

## 🚧 Roadmap

- [ ] Password hashing with BCrypt
- [ ] Leave balance tracking per employee per year
- [ ] Pagination on employee list (filter endpoint already built)
- [ ] Export attendance to Excel
- [ ] Export salary slip directly to PDF
- [ ] Email notifications on leave approval/rejection
- [ ] Dark mode toggle
- [ ] Unit tests — xUnit (.NET) + Jasmine (Angular)
- [ ] CI/CD pipeline with GitHub Actions
- [ ] Deploy to Azure App Service + Azure SQL

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
<summary><b>EF Core migration fails — table already exists</b></summary>

Your database was created manually. Create the migrations history table in SSMS first:
```sql
USE employeeManageDb;
CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId]    NVARCHAR(150) NOT NULL,
    [ProductVersion] NVARCHAR(32)  NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);
```
Then add a migration, empty its `Up()` method, and run `dotnet ef database update`.
</details>

<details>
<summary><b>Decimal precision warnings on SalaryModel</b></summary>

Add `[Column(TypeName = "decimal(18,2)")]` to every decimal property in `SalaryModel.cs`.
</details>

<details>
<summary><b>Skeleton shows forever / data never loads</b></summary>

Ensure `isLoading.set(false)` is called in **both** `next` and `error` callbacks of every subscribe. Check browser Console (F12) for API errors.
</details>

<details>
<summary><b>Check-in says "Already checked in today"</b></summary>

One check-in per employee per day is enforced. To reset for testing, delete today's row from `attendanceTbl` in SSMS.
</details>

<details>
<summary><b>400 Bad Request on Save Employee</b></summary>

Usually `designationId` sent as string `"0"`. Use `[ngValue]` not `[value]` on dropdown option elements.
</details>

<details>
<summary><b>Docker / SQL Server not connecting</b></summary>

```bash
docker ps                 # check if running
docker start sqlserver    # start if stopped
```
Then re-run the `netsh` port proxy command in Admin PowerShell.
</details>

<details>
<summary><b>Angular signal warning in template</b></summary>

Signals must be called as functions. Change `isLoading` → `isLoading()` in all HTML templates.
</details>

---

## 📋 Production Checklist

- [ ] Change SA password to a strong unique value
- [ ] Move connection string to environment variables or Azure Key Vault
- [ ] Set JWT secret key to 256-bit random value with 60-minute expiry
- [ ] Implement BCrypt password hashing
- [ ] Update CORS to production frontend domain only
- [ ] Remove or protect Swagger UI
- [ ] Update `environment.prod.ts` with production API URL
- [ ] Run `ng build --configuration production`
- [ ] Use managed SQL (Azure SQL / AWS RDS) instead of Docker
- [ ] Enable automated database backups
- [ ] Create dedicated SQL user with minimal permissions (not SA)

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