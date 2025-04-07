import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { AuthService } from '../../../../shared/services/auth-service.service';

@Component({
  selector: 'app-auth-callback',
  standalone: true,
  templateUrl: './auth-callback.component.html',
  styleUrl: './auth-callback.component.css'
})
export class AuthCallbackComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly messageService = inject(NzMessageService);
  private readonly router = inject(Router);

  ngOnInit(): void {
    const code = new URLSearchParams(window.location.search).get('code');
    if (code) {
      this.handleAuthorizationCode(code);
    } else {
      this.messageService.error('Authorization code not found.');
    }
  }

  private handleAuthorizationCode(code: string): void {
    const loading = this.messageService.loading('Logging in...', { nzDuration: 0 }).messageId;

    this.authService.exchangeAuthorizationCode(code).subscribe({
      next: (response) => {
        this.messageService.remove(loading);

        if (response) {
          this.authService.setToken(response);
          this.router.navigateByUrl('dashboard');
        } else {
          this.messageService.error('Token exchange failed.');
        }
      },
      error: (err) => {
        this.messageService.remove(loading);
        console.error('Token exchange error:', err);
        this.messageService.error('Login failed. Please try again.');
      }
    });
  }
}
