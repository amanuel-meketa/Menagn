import { AuthService } from '../shared/services/auth-service.service';
import { firstValueFrom, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

export function appInitializer(authService: AuthService) {
  return () => {
    return firstValueFrom(authService.loadCurrentUser().pipe(
        catchError(() => of(null))
      )
    );
  };
}