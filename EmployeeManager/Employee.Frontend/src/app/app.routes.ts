import { Routes } from '@angular/router';
import { Login } from './auth/login/login';
import { Attendance } from './common/attendance/attendance';
import { Leaves } from './common/leaves/leaves';
import { Tasks } from './common/tasks/tasks';
import { Dashboard } from './employee/dashboard/dashboard';
import { MyAnnouncements } from './employee/my-announcements/my-announcements';
import { MyProfile } from './employee/my-profile/my-profile';
import { SalarySlip } from './employee/salary-slips/salary-slips';
import { Announcements } from './hrAdmin/announcements/announcements';
import { Department } from './hrAdmin/department/department';
import { Designation } from './hrAdmin/designation/designation';
import { EmployeeForm } from './hrAdmin/employee-form/employee-form';
import { EmployeeList } from './hrAdmin/employee-list/employee-list';
import { Salary } from './hrAdmin/salary/salary';
import { Header } from './layout/header/header';
import { authGuard, hrGuard } from './shared/guards/auth.guard';
import { ChatComponent } from './common/chat/chat';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  {
    path: '',
    component: Header,
    canActivate: [authGuard],
    children: [
      // HR routes
      { path: 'dashboard',        component: Dashboard,       canActivate: [hrGuard] },
      { path: 'new-employee',     component: EmployeeForm,    canActivate: [hrGuard] },
      { path: 'new-employee/:id', component: EmployeeForm,    canActivate: [hrGuard] },
      { path: 'employees',        component: EmployeeList,    canActivate: [hrGuard] },
      { path: 'departments',      component: Department,      canActivate: [hrGuard] },
      { path: 'designations',     component: Designation,     canActivate: [hrGuard] },
      { path: 'salary',           component: Salary,          canActivate: [hrGuard] },
      { path: 'announcements',    component: Announcements,   canActivate: [hrGuard] },
      // Both roles
      { path: 'tasks',            component: Tasks },
      { path: 'leaves',           component: Leaves },
      { path: 'attendance',       component: Attendance },
      { path : 'chat',            component: ChatComponent},
      // Employee only
      { path: 'my-salary',        component: SalarySlip },
      { path: 'my-announcements', component: MyAnnouncements },
      { path: 'my-profile',       component: MyProfile },
    ]
  },
  { path: '**', redirectTo: 'login' }
];