import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { SessionService } from '../services/session.service';


export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const session = inject(SessionService);
  const router  = inject(Router);

  return next(req).pipe(
    catchError(err => {
      if (err.status === 401) {
        session.clear();
        router.navigate(['/login']);
      }
      return throwError(() => err);
    })
  );
};