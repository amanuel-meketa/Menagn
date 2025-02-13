import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('Bearer'); // Only `access_token` is stored

  // Clone request and set Authorization header if a valid token exists
  const authReq = token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }): req;

  return next(authReq);
};
