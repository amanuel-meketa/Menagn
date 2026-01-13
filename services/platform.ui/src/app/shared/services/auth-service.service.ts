import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, of, catchError, map } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { AuthMeResponse, BackendUser, GetCurrentUser } from '../../models/User/GetCurrentUser';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/security/api`;

  private currentUserSubject = new BehaviorSubject<GetCurrentUser | null>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  constructor() {
    this.restoreUserFromSession(); // <-- automatically restore on service init
  }      

  /** Load current user (mock in dev, real in prod) */
  loadCurrentUser(): Observable<GetCurrentUser> {
    if (!environment.production) {
      return this.loadMockUser();
    }

   return this.http.get<AuthMeResponse>(`${this.baseUrl}/auth/me`).pipe(
      map(response => {
        if (response?.user?.user) { 
          const user = this.mapBackendUser(response.user.user);
          this.setUser(user);
          return user;
        } else {
          console.warn('No user data in /me response', response);
          this.clearUser();
          return null as any;
        }
      }),
      catchError(err => {
        console.error('Failed to load /me', err);
        this.clearUser();
        throw err;
      })
    );
  }

  /** Restore user from session storage */
  private restoreUserFromSession(): void {
    try {
      const stored = sessionStorage.getItem('currentUser');
      if (stored) {
        const parsed: GetCurrentUser = JSON.parse(stored);
        // simple validation
        if (parsed.userId && parsed.username) {
          this.currentUserSubject.next(parsed);
          console.log('Restored user from session:', parsed);
        } else {
          this.clearUser();
        }
      }
    } catch (err) {
      console.error('Failed to restore user from session', err);
      this.clearUser();
    }
  }

  /** Logout */
  logout(): void {
    this.clearUser();
    window.location.href = `${this.baseUrl}/logout`;
  }

  /** Access current user synchronously */
  get currentUser(): GetCurrentUser | null {
    return this.currentUserSubject.value;
  }

  // -----------------------
  // Private helpers
  // -----------------------

  private mapBackendUser(u: BackendUser): GetCurrentUser {
    const firstName = u.given_name?.trim() || undefined;
    const lastName = u.family_name?.trim() || undefined;
    const fullName = u.name?.trim() || `${firstName || ''} ${lastName || ''}`.trim();

    return {
      userId: u.sub,
      email: u.email,
      username: u.preferred_username || u.name,
      fullName,
      firstName,
      lastName
    };
  }

  private loadMockUser(): Observable<GetCurrentUser> {
    const mockUser: GetCurrentUser = {
      userId: '338e9b83-b214-453c-af4f-a468ffce6219',
      username: 'user (mock)',
      email: 'user@debelo.com',
      fullName: 'Debelo User',
      firstName: 'Debelo',
      lastName: 'User'
    };
    this.setUser(mockUser);
    return of(mockUser);
  }

  private setUser(user: GetCurrentUser): void {
    this.currentUserSubject.next(user);
    sessionStorage.setItem('currentUser', JSON.stringify(user));
    console.log('User set in session:', user); // debug log
  }

  private clearUser(): void {
    sessionStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }
}
