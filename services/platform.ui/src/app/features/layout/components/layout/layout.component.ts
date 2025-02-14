import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzBreadCrumbModule } from 'ng-zorro-antd/breadcrumb';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { AuthService } from '../../../../shared/services/auth-service.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    RouterOutlet, RouterLink, CommonModule, NzBreadCrumbModule, NzIconModule, NzLayoutModule,
    NzMenuModule, NzButtonModule, NzDropDownModule
  ],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css'
})
export class LayoutComponent {
  private readonly _authService = inject(AuthService); 
  private router = inject(Router);
  isCollapsed = false;
  currentYear: number = new Date().getFullYear();

  token = this._authService.getStoredToken()?.emailAddress;

  onPasswordReset(): void {
    console.log('Password Reset clicked');
  }

  onUserProfile(): void {
    console.log('User Profile clicked');
  }

  onLogout(): void {
    this._authService.logout()
    this.router.navigateByUrl('/login');
  }
}