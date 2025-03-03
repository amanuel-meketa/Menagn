import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { UserService } from '../../features/user/services/user.service';
import { GetCurrentUser } from '../../models/User/GetCurrentUser';
import { Auth } from '../model/auth';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly _userService = inject(UserService);

  private setToken(authData: Auth): void {
    sessionStorage.setItem('Bearer', JSON.stringify(authData));
  }

  public getStoredToken(): Auth | null {
    const stored = sessionStorage.getItem('Bearer');
    return stored ? JSON.parse(stored) : null;
  }

  removeToken(): void {
    sessionStorage.removeItem('Bearer');
    localStorage.removeItem('columnSettings');
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

  logout(): void {
    this.removeToken();
  }

  /**
   * Authenticate and set user details in storage
   */
  authenticateUser(loginResponse: Partial<Auth>): Observable<GetCurrentUser> {
    if (!loginResponse.access_token || !loginResponse.refresh_token) {
      throw new Error('Invalid login response: Missing required authentication tokens.');
    }

    // Ensure that the `Auth` data is properly typed
    const authData: Auth = {
      access_token: loginResponse.access_token!,
      refresh_token: loginResponse.refresh_token!,
      token_type: loginResponse.token_type ?? 'Bearer',
      expires_in: loginResponse.expires_in ?? '240',
      nameIdentifier: '', 
      emailAddress: '' 
    };

    // Store token before fetching user
    this.setToken(authData);

    return this.getCurrentUser().pipe(
      tap(user => {
        if (user) {
          // Update `authData` with user details
          authData.nameIdentifier = user.nameIdentifier;
          authData.emailAddress = user.emailAddress;

          this.setToken(authData);
          this.setCurrentUser(user);
        }
      })
    );
  }
}
