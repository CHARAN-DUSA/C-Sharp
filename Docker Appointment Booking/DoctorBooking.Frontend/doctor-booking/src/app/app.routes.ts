import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { patientGuard } from './core/guards/patient.guard';
import { doctorGuard } from './core/guards/doctor.guard';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./modules/public/home/home.component').then(m => m.HomeComponent) },
  { path: 'doctors', loadComponent: () => import('./modules/public/doctor-listing/doctor-listing.component').then(m => m.DoctorListingComponent) },
  { path: 'doctors/:id', loadComponent: () => import('./modules/public/doctor-details/doctor-details.component').then(m => m.DoctorDetailsComponent) },
  { path: 'auth/login', loadComponent: () => import('./modules/auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'auth/register', loadComponent: () => import('./modules/auth/register/register.component').then(m => m.RegisterComponent) },
  { path: 'auth/forgot-password', loadComponent: () => import('./modules/auth/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent) },
  { path: 'patient/my-appointments', canActivate: [patientGuard], loadComponent: () => import('./modules/patient/my-appointments/my-appointments.component').then(m => m.MyAppointmentsComponent) },
  { path: 'patient/history', canActivate: [patientGuard], loadComponent: () => import('./modules/patient/appointment-history/appointment-history.component').then(m => m.AppointmentHistoryComponent) },
  { path: 'patient/book/:doctorId', canActivate: [patientGuard], loadComponent: () => import('./modules/patient/book-appointment/book-appointment.component').then(m => m.BookAppointmentComponent) },
  { path: 'patient/profile', canActivate: [patientGuard], loadComponent: () => import('./modules/patient/patient-profile/patient-profile.component').then(m => m.PatientProfileComponent) },
  { path: 'doctor/dashboard', canActivate: [doctorGuard], loadComponent: () => import('./modules/doctor/doctor-dashboard/doctor-dashboard.component').then(m => m.DoctorDashboardComponent) },
  { path: 'doctor/appointments', canActivate: [doctorGuard], loadComponent: () => import('./modules/doctor/doctor-appointments/doctor-appointments.component').then(m => m.DoctorAppointmentsComponent) },
  { path: 'doctor/availability', canActivate: [doctorGuard], loadComponent: () => import('./modules/doctor/manage-availability/manage-availability.component').then(m => m.ManageAvailabilityComponent) },
  { path: 'doctor/profile', canActivate: [doctorGuard], loadComponent: () => import('./modules/doctor/doctor-profile/doctor-profile.component').then(m => m.DoctorProfileComponent) },
  { path: 'admin/dashboard', canActivate: [adminGuard], loadComponent: () => import('./modules/admin/admin-dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent) },
  { path: 'admin/users', canActivate: [adminGuard], loadComponent: () => import('./modules/admin/manage-users/manage-users.component').then(m => m.ManageUsersComponent) },
  { path: 'admin/doctors', canActivate: [adminGuard], loadComponent: () => import('./modules/admin/manage-doctors/manage-doctors.component').then(m => m.ManageDoctorsComponent) },
  { path: 'admin/appointments', canActivate: [adminGuard], loadComponent: () => import('./modules/admin/view-appointments/view-appointments.component').then(m => m.ViewAppointmentsComponent) },
  { path: 'chat', canActivate: [authGuard], loadComponent: () => import('./modules/communication/chat/chat.component').then(m => m.ChatComponent) },
  { path: 'notifications', canActivate: [authGuard], loadComponent: () => import('./modules/communication/notifications/notifications.component').then(m => m.NotificationsComponent) },
  {
  path: 'auth/callback',
  loadComponent: () =>
    import('./modules/auth/callback/callback.component')
      .then(m => m.CallbackComponent)
},
  { path: 'reviews', canActivate: [authGuard], loadComponent: () => import('./modules/communication/reviews/reviews.component').then(m => m.ReviewsComponent) },
  { path: '**', redirectTo: '' }
];
