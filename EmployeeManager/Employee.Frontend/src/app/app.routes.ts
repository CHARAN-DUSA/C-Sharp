import { Routes } from '@angular/router';
import { authGuard, hrGuard } from './guards/auth.guard';
import { Login }            from './pages/login/login';
import { Header }           from './pages/header/header';
import { Dashboard }        from './pages/dashboard/dashboard';
import { EmployeeForm }     from './pages/employee-form/employee-form';
import { EmployeeList }     from './pages/employee-list/employee-list';
import { Department }       from './pages/department/department';
import { Designation }      from './pages/designation/designation';
import { MyProfile }        from './pages/my-profile/my-profile';
import { Salary }           from './pages/salary/salary';
import { Tasks }            from './pages/tasks/tasks';
import { Announcements }    from './pages/announcements/announcements';
import { Leaves }           from './pages/leaves/leaves';
import { Attendance }       from './pages/attendance/attendance';
import { SalarySlip }       from './pages/salary-slips/salary-slips';
import { MyAnnouncements }  from './pages/my-announcements/my-announcements';

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
      // Employee only
      { path: 'my-salary',        component: SalarySlip },
      { path: 'my-announcements', component: MyAnnouncements },
      { path: 'my-profile',       component: MyProfile },
    ]
  },
  { path: '**', redirectTo: 'login' }
];