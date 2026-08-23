import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../../AdminServices/AuthService/authService';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const token = authService.getAccessToken();

  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: "Bearer "+token
      }
    });
  }
  return next(req).pipe(

    catchError((error: HttpErrorResponse) => {

      if (error.status === 401) {
        authService.isloggedIn.set(false);
        router.navigate(['/login']);
      }

      return throwError(() => error);
    })

  );

};
