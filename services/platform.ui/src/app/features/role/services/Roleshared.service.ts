import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { GetRole } from '../../../models/User/GetUserRole';

@Injectable({
  providedIn: 'root',
})
export class RolesharedService {
    private currentRoleSubject = new BehaviorSubject<GetRole | null>(null); // Holds the current role
    currentRole$ = this.currentRoleSubject.asObservable(); // Expose it as an observable  
  
    setCurrentRole(role: GetRole): void {
      this.currentRoleSubject.next(role);
    }
  }