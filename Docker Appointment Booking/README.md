<div align="center">

# 💊 MediBook — Doctor Appointment Booking System

[![Angular](https://img.shields.io/badge/Angular-17-DD0031?style=for-the-badge&logo=angular&logoColor=white)](https://angular.io/)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Azure SQL](https://img.shields.io/badge/Azure_SQL-0078D4?style=for-the-badge&logo=microsoftazure&logoColor=white)](https://azure.microsoft.com/en-us/products/azure-sql/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![Firebase](https://img.shields.io/badge/Firebase-FFCA28?style=for-the-badge&logo=firebase&logoColor=black)](https://firebase.google.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

**A production-grade, full-stack doctor appointment booking platform built for a hackathon.**  
Patients book appointments, doctors manage schedules, and the system prevents double-booking with **RowVersion concurrency** — the star feature.

[📸 Screenshots](#screenshots) • [🚀 Getting Started](#getting-started) • [🏗️ Architecture](#architecture) • [✨ Features](#features) • [🔧 Tech Stack](#tech-stack) • [📡 API Reference](#api-reference)

---

![MediBook Banner](https://via.placeholder.com/1200x400/1e4d8c/ffffff?text=MediBook+%E2%80%94+Doctor+Appointment+Booking+System)

</div>

---

## 📋 Table of Contents

- [About the Project](#about-the-project)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Environment Variables](#environment-variables)
- [API Reference](#api-reference)
- [Authentication Flow](#authentication-flow)
- [Database Schema](#database-schema)
- [Pages & Roles](#pages--roles)
- [Email Notifications](#email-notifications)
- [Concurrency Handling](#concurrency-handling)
- [Running Tests](#running-tests)
- [Deployment](#deployment)
- [Screenshots](#screenshots)
- [Contributing](#contributing)
- [License](#license)

---

## 🎯 About the Project

**MediBook** is a real-world doctor appointment booking system that demonstrates enterprise-level software engineering practices. Built during a hackathon, it covers the full lifecycle of patient–doctor interaction:

- Patients search for doctors by specialty, read reviews, and book time slots
- Doctors manage their availability, view appointments, and chat with patients
- Admins oversee the platform — verify doctors, manage users, and view analytics
- The system **prevents double-booking** using SQL Server's `RowVersion` concurrency token
- **Email notifications** are sent on account creation, appointment booking, and as reminders
- **Real-time chat** auto-refreshes every 3 seconds — no manual refresh needed
- **Three authentication methods**: Email/Password, Google OAuth, and Firebase OTP

---

## ✨ Features

### 🔐 Authentication & Security
| Feature | Details |
|---|---|
| JWT Authentication | Role-based access with 60-minute token expiry |
| Google OAuth 2.0 | Server-side token verification via `Google.Apis.Auth` |
| Firebase OTP | Phone number login and password reset via Firebase SMS |
| Two-Factor Authentication | TOTP (RFC 6238) compatible with Google Authenticator / Authy |
| Password Reset | Email OTP (6-digit, 10-minute expiry, 3-attempt lockout) |
| IP Rate Limiting | Per-endpoint rate limiting via `AspNetCoreRateLimit` |
| Account Lockout | Built-in Identity lockout after failed login attempts |

### 📅 Appointment Management
| Feature | Details |
|---|---|
| Real-time Slot Availability | Doctors generate time slots for specific dates |
| RowVersion Concurrency | Prevents two patients from booking the same slot simultaneously |
| Appointment Lifecycle | Pending → Confirmed → Completed / Cancelled / NoShow |
| Auto No-Show Detection | Background service marks missed appointments after 2 hours |
| Cancellation | Patients can cancel with RowVersion validation |
| Slot Auto-Expiry | Expired unbooked slots are automatically deleted daily |

### 💬 Communication
| Feature | Details |
|---|---|
| Real-time Chat | Auto-polls every 3 seconds + SignalR WebSocket fallback |
| All Doctors Available | Patients can start a chat with any registered doctor |
| Unread Badge | Navbar shows live unread message count (polls every 10s) |
| Notifications | In-app notifications for appointments, messages, and reminders |
| Email Notifications | Welcome email, booking confirmation, day-before reminder |

### 👥 Multi-Role System
| Role | Access |
|---|---|
| **Patient** | Book appointments, chat, view history, write reviews |
| **Doctor** | Manage availability, view bookings, complete appointments, chat |
| **Admin** | Verify doctors, manage all users, view all appointments, analytics |

### 🏥 Doctor Discovery
| Feature | Details |
|---|---|
| Search & Filter | Search by name or specialty, filter by availability |
| Doctor Profiles | Rating, reviews, experience, qualifications, clinic info |
| Specialty Browse | 8 specialty categories on the home page |
| Star Ratings | Aggregated from patient reviews |

### 🛠️ Engineering Highlights
| Feature | Details |
|---|---|
| Soft Delete | Global query filter — deleted records never returned |
| ProblemDetails (RFC 7807) | Standardized error responses across all endpoints |
| FluentValidation | All DTOs validated with custom messages |
| Background Service | `IHostedService` running hourly cleanup + reminders |
| Lazy Loading | Angular routes are lazy-loaded for performance |
| Signals | Angular 17 Signals for reactive state management |
| Standalone Components | No NgModules — pure standalone component architecture |

---

## 🔧 Tech Stack

### Frontend
```
Angular 17          — Standalone components, Signals, Lazy loading
Bootstrap 5.3       — UI framework + Bootstrap Icons
TypeScript 5.2      — Strict mode enabled
RxJS 7.8            — Reactive state management
@microsoft/signalr  — Real-time WebSocket client
Firebase JS SDK     — Phone OTP + Google popup auth
```

### Backend
```
ASP.NET Core 8.0    — Web API, minimal hosting model
Entity Framework 8  — ORM with migrations
ASP.NET Identity    — User management + role-based auth
SignalR             — Real-time chat hub
MailKit             — Email via Gmail SMTP
Google.Apis.Auth    — Server-side Google token verification
FirebaseAdmin REST  — Firebase token verification
FluentValidation    — Request validation
AspNetCoreRateLimit — IP-based rate limiting
Swashbuckle         — Swagger/OpenAPI docs
```

### Database & Cloud
```
Azure SQL Database  — Production database (SQL Server)
Entity Framework    — Code-first migrations
RowVersion          — Optimistic concurrency control
```

### DevOps & Testing
```
xUnit               — Integration test framework
WebApplicationFactory — In-memory test host
```

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Angular 17 Frontend                      │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
│  │  Patient │  │  Doctor  │  │  Admin   │  │  Public  │   │
│  │  Pages   │  │  Pages   │  │  Pages   │  │  Pages   │   │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘   │
│  ┌────────────────────────────────────────────────────────┐  │
│  │        Core Services (Auth, SignalR, Storage)          │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────┬──────────────────────────────────┘
                           │ HTTPS / JWT
┌──────────────────────────▼──────────────────────────────────┐
│                  ASP.NET Core 8 API                          │
│  ┌──────────┐  ┌──────────────┐  ┌───────────────────────┐  │
│  │   API    │  │ Application  │  │    Infrastructure     │  │
│  │Controllers│  │  DTOs/Valid  │  │  Repos/Services/EF   │  │
│  └──────────┘  └──────────────┘  └───────────────────────┘  │
│  ┌──────────────┐  ┌────────────┐  ┌─────────────────────┐  │
│  │  SignalR Hub │  │ Background │  │  Email/Google/      │  │
│  │  (Chat)      │  │  Service   │  │  Firebase Services  │  │
│  └──────────────┘  └────────────┘  └─────────────────────┘  │
└──────────────────────────┬──────────────────────────────────┘
                           │ EF Core
┌──────────────────────────▼──────────────────────────────────┐
│                    Azure SQL Database                        │
│  Users  │  Doctors  │  Patients  │  Appointments  │  Slots  │
│  Chat   │  Reviews  │  Notifications              │         │
└─────────────────────────────────────────────────────────────┘
```

### Clean Architecture Layers

```
DoctorBooking.API/
├── Domain/           → Entities, Enums (pure business objects)
├── Application/      → DTOs, Validators, Interfaces
├── Infrastructure/   → EF Core, Repositories, Services
└── API/              → Controllers, Middleware, SignalR Hubs
```

---

## 📁 Project Structure

```
MediBook/
├── backend/
│   └── DoctorBooking.API/
│       ├── Domain/
│       │   ├── Entities/
│       │   │   ├── AppUser.cs
│       │   │   ├── Doctor.cs
│       │   │   ├── Patient.cs
│       │   │   ├── Appointment.cs
│       │   │   ├── TimeSlot.cs
│       │   │   ├── ChatMessage.cs
│       │   │   ├── Notification.cs
│       │   │   └── Review.cs
│       │   └── Enums/
│       │       └── AppointmentStatus.cs
│       ├── Application/
│       │   ├── DTOs/
│       │   │   ├── Auth/         → LoginDto, RegisterDto, GoogleLoginDto, etc.
│       │   │   ├── Appointment/  → BookAppointmentDto, AppointmentResponseDto
│       │   │   ├── Doctor/       → DoctorAvailabilityDto, UpdateSlotDto
│       │   │   └── Common/       → PaginatedResult
│       │   ├── Interfaces/       → Repository contracts
│       │   └── Validators/       → FluentValidation validators
│       ├── Infrastructure/
│       │   ├── Data/
│       │   │   ├── AppDbContext.cs
│       │   │   └── DbSeeder.cs
│       │   ├── Repositories/
│       │   │   ├── AppointmentRepository.cs
│       │   │   ├── DoctorRepository.cs
│       │   │   ├── PatientRepository.cs
│       │   │   ├── ChatRepository.cs
│       │   │   └── NotificationRepository.cs
│       │   └── Services/
│       │       ├── EmailService.cs          → MailKit + Gmail SMTP
│       │       ├── TokenService.cs          → JWT generation
│       │       ├── GoogleAuthService.cs     → Google token verification
│       │       ├── FirebaseAdminService.cs  → Firebase REST verification
│       │       ├── TwoFactorService.cs      → TOTP (RFC 6238)
│       │       └── AppointmentCleanupService.cs  → Background job
│       ├── API/
│       │   ├── Controllers/
│       │   │   ├── AuthController.cs
│       │   │   ├── AppointmentsController.cs
│       │   │   ├── DoctorsController.cs
│       │   │   ├── PatientsController.cs
│       │   │   ├── AdminController.cs
│       │   │   ├── ChatController.cs
│       │   │   └── NotificationsController.cs
│       │   ├── Middleware/
│       │   │   └── ExceptionMiddleware.cs
│       │   └── Hubs/
│       │       └── ChatHub.cs
│       ├── Program.cs
│       └── appsettings.json
│
├── frontend/
│   └── doctor-booking/
│       └── src/
│           └── app/
│               ├── core/
│               │   ├── guards/      → auth, patient, doctor, admin
│               │   ├── interceptors/→ jwt, error
│               │   ├── models/      → user, appointment, doctor, chat
│               │   └── services/    → auth, firebase, signalr, storage
│               ├── shared/
│               │   ├── components/  → navbar, sidebar, toast, loader
│               │   └── pipes/       → time-ago
│               └── modules/
│                   ├── auth/        → login, register, forgot-password
│                   ├── public/      → home, doctor-listing, doctor-details
│                   ├── patient/     → book-appointment, my-appointments, history, profile
│                   ├── doctor/      → dashboard, appointments, availability, profile
│                   ├── admin/       → dashboard, users, doctors, appointments
│                   └── communication/ → chat, notifications, reviews
│
└── tests/
    └── DoctorBooking.IntegrationTests/
        ├── Helpers/
        │   └── TestWebAppFactory.cs
        └── Tests/
            ├── AuthTests.cs
            ├── AppointmentTests.cs
            └── ConcurrencyTests.cs   ← RowVersion star feature tests
```

---

## 🚀 Getting Started

### Prerequisites

```bash
# Check versions
node --version        # v18.0.0 or higher
dotnet --version      # 8.0.0 or higher
ng version            # Angular CLI 17+
```

Install Angular CLI if needed:
```bash
npm install -g @angular/cli@17
```

---

### 1. Clone the Repository

```bash
git clone https://github.com/CHARAN-DUSA/C-Sharp.git
cd medibook
```

---

### 2. Backend Setup

```bash
cd backend/DoctorBooking.API
```

#### Install NuGet packages
```bash
dotnet restore
```

#### Configure `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:YOUR_SERVER.database.windows.net,1433;Initial Catalog=YOUR_DB;User ID=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=True;"
  },
  "JwtSettings": {
    "Secret": "YourSuperSecretKeyHereMustBe32CharsMin!",
    "Issuer": "DoctorBookingAPI",
    "Audience": "DoctorBookingClient",
    "ExpirationInMinutes": 60
  },
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "your-email@gmail.com",
    "Password": "YOUR_16_CHAR_APP_PASSWORD",
    "FromName": "MediBook",
    "FromEmail": "your-email@gmail.com"
  },
  "GoogleAuth": {
    "ClientId": "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com",
    "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
  },
  "Firebase": {
    "ProjectId": "your-firebase-project-id",
    "ApiKey": "YOUR_FIREBASE_API_KEY",
    "AuthDomain": "your-project.firebaseapp.com"
  }
}
```

#### Run migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### Start the backend
```bash
dotnet run
# API runs on https://localhost:7001
# Swagger UI: https://localhost:7001/swagger
```

The **DbSeeder** automatically creates:
- 3 roles: Patient, Doctor, Admin
- 1 demo admin, 1 demo patient, 6 demo doctors

---

### 3. Frontend Setup

```bash
cd frontend/doctor-booking
npm install
```

#### Configure `src/environments/environment.ts`
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7001/api',
  hubUrl: 'https://localhost:7001',
  googleClientId: 'YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com',
  firebaseConfig: {
    apiKey:     'YOUR_FIREBASE_API_KEY',
    authDomain: 'your-project.firebaseapp.com',
    projectId:  'your-firebase-project-id'
  }
};
```

#### Start the frontend
```bash
ng serve
# Opens at http://localhost:4200
```

---

### 4. Demo Credentials

| Role | Email | Password |
|---|---|---|
| 👤 Patient | patient@demo.com | Demo@1234 |
| 👨‍⚕️ Doctor | doctor@demo.com | Demo@1234 |
| 🛡️ Admin | admin@demo.com | Demo@1234 |

---

## 🔑 Environment Variables

### Google OAuth Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com/apis/credentials)
2. Create an **OAuth 2.0 Client ID** (Web application)
3. Add **Authorized JavaScript origins**:
   ```
   http://localhost:4200
   http://localhost
   ```
4. Add **Authorized redirect URIs**:
   ```
   http://localhost:4200
   http://localhost:4200/auth/login
   ```

### Gmail App Password

> ⚠️ Your regular Gmail password will NOT work. You need an App Password.

1. Go to [myaccount.google.com/security](https://myaccount.google.com/security)
2. Enable **2-Step Verification** (required)
3. Go to [App Passwords](https://myaccount.google.com/apppasswords)
4. Create app name `MediBook` → copy the 16-character password
5. Use it **without spaces** in `appsettings.json`

### Firebase Setup

1. Go to [Firebase Console](https://console.firebase.google.com)
2. Create a project (or use existing)
3. Enable **Authentication** → **Phone** provider
4. Enable **Authentication** → **Google** provider
5. Under **Authentication → Settings → Authorized domains**, add `localhost`
6. Copy API key, Project ID, and Auth Domain to your config

---

## 📡 API Reference

### Base URL
```
https://localhost:7001/api
```

### Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|---|---|---|---|
| `POST` | `/auth/register` | Register with email/password | ❌ |
| `POST` | `/auth/login` | Login with email/password | ❌ |
| `POST` | `/auth/google` | Login with Google ID token | ❌ |
| `POST` | `/auth/firebase-login` | Login with Firebase token (OTP/phone) | ❌ |
| `POST` | `/auth/forgot-password/send-otp` | Send 6-digit reset OTP to email | ❌ |
| `POST` | `/auth/forgot-password/verify-otp` | Verify OTP and set new password | ❌ |
| `POST` | `/auth/2fa/setup` | Generate TOTP secret + QR code | ✅ |
| `POST` | `/auth/2fa/enable` | Enable 2FA after scanning QR | ✅ |
| `POST` | `/auth/2fa/disable` | Disable 2FA | ✅ |
| `POST` | `/auth/2fa/verify` | Verify TOTP code at login | ❌ |

### Doctor Endpoints

| Method | Endpoint | Description | Auth Required |
|---|---|---|---|
| `GET` | `/doctors` | List all doctors (search + filter) | ❌ |
| `GET` | `/doctors/{id}` | Get doctor profile | ❌ |
| `GET` | `/doctors/{id}/slots` | Get available slots for a date | ❌ |
| `GET` | `/doctors/specialties` | Get all specialty names | ❌ |
| `POST` | `/doctors/availability` | Generate slots for a single day | 🩺 Doctor |
| `GET` | `/doctors/my-slots` | Get my slots by date | 🩺 Doctor |
| `GET` | `/doctors/my-profile` | Get my doctor profile | 🩺 Doctor |
| `PUT` | `/doctors/my-profile` | Update my doctor profile | 🩺 Doctor |
| `DELETE` | `/doctors/slots/{id}` | Cancel an unbooked slot | 🩺 Doctor |
| `PATCH` | `/doctors/slots/{id}` | Adjust slot time | 🩺 Doctor |
| `GET` | `/doctors/stats` | Get my dashboard stats | 🩺 Doctor |

### Appointment Endpoints

| Method | Endpoint | Description | Auth Required |
|---|---|---|---|
| `POST` | `/appointments` | Book an appointment | 👤 Patient |
| `GET` | `/appointments/my` | Get my appointments (filterable) | ✅ |
| `GET` | `/appointments/{id}` | Get appointment details | ✅ |
| `PATCH` | `/appointments/{id}/cancel` | Cancel appointment (RowVersion) | ✅ |
| `PATCH` | `/appointments/{id}/confirm` | Confirm appointment | 🩺 Doctor |
| `PATCH` | `/appointments/{id}/complete` | Complete appointment | 🩺 Doctor |

### Chat Endpoints

| Method | Endpoint | Description | Auth Required |
|---|---|---|---|
| `GET` | `/chat/conversations` | Get all conversations | ✅ |
| `GET` | `/chat/messages/{userId}` | Get messages with a user | ✅ |
| `POST` | `/chat/send` | Send a message | ✅ |
| `PATCH` | `/chat/read/{userId}` | Mark messages as read | ✅ |

### Admin Endpoints

| Method | Endpoint | Description | Auth Required |
|---|---|---|---|
| `GET` | `/admin/stats` | Platform statistics | 🛡️ Admin |
| `GET` | `/admin/users` | All users (paginated) | 🛡️ Admin |
| `PATCH` | `/admin/users/{id}/toggle-status` | Enable/disable a user | 🛡️ Admin |
| `GET` | `/admin/doctors` | All doctors (paginated) | 🛡️ Admin |
| `PATCH` | `/admin/doctors/{id}/verify` | Verify a doctor | 🛡️ Admin |
| `GET` | `/admin/appointments` | All appointments (paginated + filtered) | 🛡️ Admin |

> **Swagger UI** available at `https://localhost:7001/swagger` when running in development.

---

## 🔐 Authentication Flow

```
Email/Password Login:
  1. POST /auth/login → { token, role, ... }
  2. If 2FA enabled → { requiresTwoFactor: true, userId }
  3. POST /auth/2fa/verify → { token, ... }

Google OAuth:
  1. Frontend: google.accounts.id.initialize() + prompt()
  2. Google returns credential (ID token)
  3. POST /auth/google { idToken } → backend verifies with Google.Apis.Auth
  4. Returns JWT

Firebase Phone OTP:
  1. Frontend: Firebase signInWithPhoneNumber()
  2. User enters 6-digit OTP
  3. Firebase confirms → returns Firebase ID token
  4. POST /auth/firebase-login { idToken } → backend verifies with Firebase REST API
  5. Returns JWT

Password Reset (Email OTP):
  1. POST /auth/forgot-password/send-otp { email }
  2. Backend generates 6-digit OTP → sends via Gmail SMTP
  3. User enters OTP + new password
  4. POST /auth/forgot-password/verify-otp { email, otp, newPassword }
```

---

## 🗄️ Database Schema

```sql
AppUsers          ── Identity user + FirstName, LastName, Role, GoogleId, FirebaseUid, AuthProvider
Doctors           ── UserId, Specialty, Qualifications, Fee, Rating, IsVerified, IsAvailable
Patients          ── UserId, Gender, DateOfBirth, BloodGroup, Allergies, MedicalHistory
Appointments      ── PatientId, DoctorId, Date, TimeSlot, Status, RowVersion (concurrency!)
TimeSlots         ── DoctorId, Date, StartTime, EndTime, IsBooked, IsBlocked
ChatMessages      ── SenderId, ReceiverId, Content, SentAt, IsRead
Notifications     ── UserId, Title, Message, Type, IsRead
Reviews           ── PatientId, DoctorId, AppointmentId, Rating, Comment
```

### RowVersion Concurrency (Hackathon Star Feature ⭐)
```csharp
// Two patients try to book the same slot simultaneously:
// Patient A reads slot → RowVersion = 0x0001
// Patient B reads slot → RowVersion = 0x0001
// Patient A books   → RowVersion = 0x0002 (success)
// Patient B tries   → EF Core throws DbUpdateConcurrencyException
//                   → Returns 409 Conflict to Patient B
```

---

## 📧 Email Notifications

MediBook sends 4 types of emails via Gmail SMTP (MailKit):

| Trigger | Email Sent |
|---|---|
| New account created | 🎉 Welcome email with role-specific onboarding info |
| Appointment booked | ✅ Booking confirmation with doctor, date, time, fee |
| Day before appointment | ⏰ Reminder email (sent by background service at hourly check) |
| Forgot password | 🔐 6-digit OTP valid for 10 minutes |

---

## ⚡ Concurrency Handling

The **RowVersion** feature is the engineering highlight of this project. It uses SQL Server's `timestamp`/`rowversion` column to implement **optimistic concurrency control** at the database level.

```csharp
// Entity
public byte[] RowVersion { get; set; }  // [Timestamp] attribute

// DbContext
modelBuilder.Entity<Appointment>()
    .Property(a => a.RowVersion)
    .IsRowVersion();

// Controller — passes RowVersion from frontend
try {
    await _repo.UpdateAsync(appointment);
} catch (DbUpdateConcurrencyException) {
    return Conflict(new { detail = "Slot was modified by another user. Please refresh." });
}
```

---

## 🧪 Running Tests

```bash
cd tests/DoctorBooking.IntegrationTests
dotnet test --verbosity normal
```

### Test Coverage

| Test Suite | What It Tests |
|---|---|
| `AuthTests.cs` | Register, Login, Google OAuth, 2FA setup, rate limiting |
| `AppointmentTests.cs` | Book, cancel, confirm, complete appointments |
| `ConcurrencyTests.cs` | **RowVersion double-booking prevention** |

### Key Concurrency Test
```csharp
[Fact]
public async Task TwoPatients_BookSameSlot_OnlyOneSucceeds()
{
    // Both patients read the same slot
    // Both try to book simultaneously
    // Assert: exactly one 200 OK, one 409 Conflict
}
```

---

## 📱 Pages & Roles

### Public Pages (No login required)
| Page | Route | Description |
|---|---|---|
| Home | `/` | Landing page with specialty browse + search |
| Find Doctors | `/doctors` | Search and filter doctors |
| Doctor Profile | `/doctors/:id` | Doctor details + reviews + book |
| Login | `/auth/login` | Email / Google / Phone OTP |
| Register | `/auth/register` | Patient or Doctor registration |
| Forgot Password | `/auth/forgot-password` | Email OTP / Phone OTP / Google |

### Patient Pages 👤
| Page | Route | Description |
|---|---|---|
| My Appointments | `/patient/my-appointments` | Upcoming confirmed appointments |
| Appointment History | `/patient/history` | Past/completed appointments |
| Book Appointment | `/patient/book/:doctorId` | Date picker + slot selection |
| Patient Profile | `/patient/profile` | Update personal + medical info |

### Doctor Pages 👨‍⚕️
| Page | Route | Description |
|---|---|---|
| Dashboard | `/doctor/dashboard` | Today's schedule + stats |
| My Appointments | `/doctor/appointments` | All appointments with status filter |
| Manage Availability | `/doctor/availability` | Generate slots + cancel/adjust |
| Doctor Profile | `/doctor/profile` | Update bio, fee, clinic info |

### Admin Pages 🛡️
| Page | Route | Description |
|---|---|---|
| Dashboard | `/admin/dashboard` | Platform stats + quick links |
| Manage Users | `/admin/users` | Enable/disable user accounts |
| Manage Doctors | `/admin/doctors` | Verify doctor accounts |
| All Appointments | `/admin/appointments` | View and filter all bookings |

### Communication Pages (All logged-in users)
| Page | Route | Description |
|---|---|---|
| Chat | `/chat` | Real-time messaging + doctor browser |
| Notifications | `/notifications` | Mark read, mark all read |
| Reviews | `/reviews` | Write and view reviews |

---

## 🚀 Deployment

### Backend — Azure App Service

```bash
# Publish
dotnet publish -c Release -o ./publish

# Update connection string for production
# Update appsettings.Production.json
# Deploy to Azure App Service
```

### Frontend — Build for Production

```bash
cd frontend/doctor-booking
ng build --configuration production
# Output in dist/doctor-booking/
```

Update `src/environments/environment.prod.ts`:
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-app.azurewebsites.net/api',
  hubUrl: 'https://your-app.azurewebsites.net',
  googleClientId: 'YOUR_GOOGLE_CLIENT_ID',
  firebaseConfig: { /* production config */ }
};
```

---

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch: `git checkout -b feature/AmazingFeature`
3. Commit your changes: `git commit -m 'Add some AmazingFeature'`
4. Push to the branch: `git push origin feature/AmazingFeature`
5. Open a Pull Request

---

## 📜 License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

Built with ❤️ for a hackathon.

---

<div align="center">

**⭐ Star this repo if you found it helpful!**

[![GitHub stars](https://img.shields.io/github/stars/yourusername/medibook?style=social)](https://github.com/yourusername/medibook)

</div>
