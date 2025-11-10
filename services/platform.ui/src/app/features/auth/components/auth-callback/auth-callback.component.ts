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
  private readonly router = inject(Router);
  private readonly messageService = inject(NzMessageService);

  ngOnInit(): void {
    // 1️⃣ Try to get token from query param (redirect from Kong)
    const params = new URLSearchParams(window.location.search);
    let token = params.get('access_token');
    const refresh = params.get('refresh_token');
    const expires_in = params.get('expires_in');

    // 2️⃣ Fallback: try to get token from a custom header set by Kong
    if (!token) {
      token = (window as any)?.KONG_ACCESS_TOKEN; // if you inject it via JS on page
    }

    if (token) {
      this.authService.setTokenFromKong({
        access_token: token,
        refresh_token: refresh || '',
        token_type: 'Bearer',
        expires_in: expires_in || '',
        nameIdentifier: '',
        emailAddress: '',
      });

      this.router.navigateByUrl('dashboard');
    } else {
      this.messageService.error('Access token not found.');
    }
  }
}
