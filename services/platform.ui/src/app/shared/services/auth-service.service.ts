import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, of, tap, catchError } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { GetCurrentUser } from '../../models/User/GetCurrentUser';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly apiRoot = environment.apiBaseUrl;
  private readonly securityApi = `${this.apiRoot}/security/api`;

  private currentUserSubject = new BehaviorSubject<GetCurrentUser | null>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  /** Load user (mock in dev, real in prod) */
  loadCurrentUser(): Observable<GetCurrentUser> {
    if (!environment.production) {
      return this.loadMockUser();
    }

    return this.http.get<GetCurrentUser>(`${this.securityApi}/me`).pipe(
      tap(user => this.setUser(user)),
      catchError(err => {
        console.error('Failed to load /me', err);
        this.clearUser();
        throw err;
      })
    );
  }

  /** Restore user from session storage */
  restoreUserFromSession(): void {
    const stored = sessionStorage.getItem('currentUser');
    if (stored) {
      this.currentUserSubject.next(JSON.parse(stored));
    }
  }

  /** Logout */
  logout(): void {
    this.clearUser();
    window.location.href = `${this.securityApi}/logout`;
  }

  /** Sync access */
  get currentUser(): GetCurrentUser | null {
    return this.currentUserSubject.value;
  }

  // -----------------------
  // Private helpers
  // -----------------------

  private loadMockUser(): Observable<GetCurrentUser> {
    const mockUser: GetCurrentUser = {
      userId: '00ce2b8e-24a4-4af8-ace6-333bf96758db',
      username: 'user (mock)',
      email: 'user@debelo.com'
    };

    this.setUser(mockUser);
    return of(mockUser);
  }

  private setUser(user: GetCurrentUser): void {
    this.currentUserSubject.next(user);
    sessionStorage.setItem('currentUser', JSON.stringify(user));
  }

  private clearUser(): void {
    sessionStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }
}
