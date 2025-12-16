import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, of, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { GetCurrentUser } from '../../models/User/GetCurrentUser';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiBaseUrl + '/security';

  private currentUserSubject = new BehaviorSubject<GetCurrentUser | null>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  /** Load user either from real endpoint or mock */
  loadCurrentUser(): Observable<GetCurrentUser> {
    if (environment.production === false) {
      const mockUser: GetCurrentUser = {
        userId: '00ce2b8e-24a4-4af8-ace6-333bf96758db',
        username: 'user(mock)',
        email: 'user@debelo.com'
      };
      this.currentUserSubject.next(mockUser);
      sessionStorage.setItem('currentUser', JSON.stringify(mockUser));
      return of(mockUser);
    } else {
      return this.http.get<GetCurrentUser>(`${this.baseUrl}/me`).pipe(
        tap(user => {
          this.currentUserSubject.next(user);
          sessionStorage.setItem('currentUser', JSON.stringify(user));
        })
      );
    }
  }

  restoreUserFromSession(): void {
    const stored = sessionStorage.getItem('currentUser');
    if (stored) this.currentUserSubject.next(JSON.parse(stored));
  }

  logout(): void {
    sessionStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
    window.location.href = `${this.baseUrl}/logout`;
  }

  get currentUser(): GetCurrentUser | null {
    return this.currentUserSubject.value;
  }
}
