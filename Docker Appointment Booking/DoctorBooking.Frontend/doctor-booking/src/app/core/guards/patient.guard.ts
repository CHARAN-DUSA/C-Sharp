import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
export const patientGuard: CanActivateFn = () => {
  const auth = inject(AuthService); const router = inject(Router);
  if (!auth.isLoggedIn()) { router.navigate(['/auth/login']); return false; }
  if (auth.userRole() !== 'Patient') { router.navigate(['/']); return false; }
  return true;
};