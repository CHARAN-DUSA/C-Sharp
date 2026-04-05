import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const auth   = inject(AuthService);
  const router = inject(Router);

  // ✅ Check signal AND localStorage to avoid race condition
  // after OAuth callback where signal may not have updated yet
  const isLoggedIn = auth.isLoggedIn() || !!localStorage.getItem('mb_token');

  if (isLoggedIn) return true;

  router.navigate(['/auth/login'], {
    queryParams: { returnUrl: state.url }
  });
  return false;
};