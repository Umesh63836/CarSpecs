import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../AdminServices/AuthService/authService';
import { inject } from '@angular/core';

export const authguardGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isloggedIn()) {
    return true;
  }

  router.navigate(['/login']);
  return false;
};
