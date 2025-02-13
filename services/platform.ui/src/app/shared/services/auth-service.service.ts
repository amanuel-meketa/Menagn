import { Injectable, signal } from '@angular/core';
import { Auth } from '../model/auth';

@Injectable({
  providedIn: 'root'
})
export class AuthService{
currentUserSig = signal<string | undefined | null>(undefined);
  constructor() { }
}
