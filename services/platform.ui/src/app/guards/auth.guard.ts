import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../shared/services/auth-service.service';

export const authGuard: CanActivateFn = (route, state) => {
  const _authService = inject(AuthService); 
  const token = _authService.getStoredToken()?.access_token;

  const router = inject(Router);

  if (!token) {
    router.navigateByUrl('login');
    return false;
  }
  
  return true;
};
