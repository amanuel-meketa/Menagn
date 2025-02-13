import { Injectable, signal } from '@angular/core';
@Injectable({
  providedIn: 'root'
})

export class AuthService { 
  currentUserSig = signal<string | undefined | null>(undefined);
}
