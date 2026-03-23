import { Injectable } from '@angular/core';
import { EmployeeModel } from '../models/Employee.model';

@Injectable({ providedIn: 'root' })
export class SessionService
{

  private readonly USER_KEY = 'empLoginUser';

  setUser(user: EmployeeModel): void
  {
    sessionStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  getUser(): EmployeeModel | null
  {
    const raw = sessionStorage.getItem(this.USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }

  getUserId(): number
  {
    return this.getUser()?.employeeId ?? 0;
  }

  getRole(): string
  {
    return this.getUser()?.role ?? '';
  }

  isLoggedIn(): boolean
  {
    return !!this.getUser();
  }

  isHR(): boolean {
  const role = this.getRole().toLowerCase().trim();
  return role === 'hr' || role === 'admin';
}

  clear(): void
  {
    sessionStorage.removeItem(this.USER_KEY);
  }
}