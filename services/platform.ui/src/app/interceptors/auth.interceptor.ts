import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../shared/services/auth-service.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const _authService = inject(AuthService); 
  const token = _authService.getStoredToken()?.access_token; 
  const tokenType = _authService.getStoredToken()?.token_type; 
  // Clone request and add Authorization header if token exists
  const authReq = token ? req.clone({ setHeaders: { Authorization: `${tokenType} ${token}` } }) : req;

  return next(authReq);
};
