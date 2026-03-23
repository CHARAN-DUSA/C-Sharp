import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { SessionService } from '../services/session.service';


export const authGuard: CanActivateFn = () => {
  const session = inject(SessionService);
  const router  = inject(Router);
  if (!session.isLoggedIn()) {
    router.navigate(['/login']);
    return false;
  }
  return true;
};

export const hrGuard: CanActivateFn = () => {
  const session = inject(SessionService);
  const router  = inject(Router);

  if (!session.isLoggedIn()) {
    return router.createUrlTree(['/login']);
  }

  // ✅ HR + Admin access
  if (!session.isHR()) {
    return router.createUrlTree(['/my-profile']);
  }

  return true;
};