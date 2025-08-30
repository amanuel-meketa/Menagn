import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { UserService } from '../../features/user/services/user.service';
import { GetCurrentUser } from '../../models/User/GetCurrentUser';
import { Auth } from '../model/auth';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly baseUrl = 'http://localhost/api';
  private readonly _userService = inject(UserService);
  private readonly http = inject(HttpClient);

  authenticateUser(): void {
    window.location.href = `${this.baseUrl}/auth/authenticate`;
  }

  exchangeAuthorizationCode(code: string): Observable<any> {
    const params = new HttpParams().set('code', code);
    return this.http.get<any>(`${this.baseUrl}/auth/exchange-token`, { params });
  }
  
  setToken(authData: any): void {
    sessionStorage.setItem('Bearer', JSON.stringify(authData));
  }

  public getStoredToken(): Auth | null {
    const stored = sessionStorage.getItem('Bearer');
    return stored ? JSON.parse(stored) : null;
  }

  removeToken(): void {
    sessionStorage.removeItem('Bearer');
    this.currentUserSubject.next(null);
  }

  // BehaviorSubject for user state
  private currentUserSubject = new BehaviorSubject<GetCurrentUser | null>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  setCurrentUser(user: GetCurrentUser): void {
    this.currentUserSubject.next(user);
  }

  getCurrentUser(): Observable<GetCurrentUser> {
    return this._userService.getCurrentUser();
  }

  public getCurrentUserInfoFromToken(): { id: string; email?: string; username?: string } | null {
  const token = sessionStorage.getItem('Bearer');
  if (!token) return null;

  try {
    const parsed = JSON.parse(token);
    const accessToken = parsed.access_token || parsed.token || parsed;
    const payloadBase64 = accessToken.split('.')[1];
    if (!payloadBase64) return null;

    const payload = JSON.parse(atob(payloadBase64));
    return {
      id: payload.sub || payload.id || '',
      email: payload.email,
      username: payload.username
    };
    
  } catch (err) {
    console.error('Error parsing token:', err);
    return null;
  }
}
  
  logout(): void {
    this.removeToken();
  }
}
