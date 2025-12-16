import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink } from '@angular/router';
import { NzBreadCrumbModule } from 'ng-zorro-antd/breadcrumb';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { AuthService } from '../../../../shared/services/auth-service.service';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [ CommonModule, RouterOutlet, RouterLink, NzBreadCrumbModule, NzIconModule, NzLayoutModule, NzMenuModule,
             NzButtonModule, NzDropDownModule],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css'
})
export class LayoutComponent {

  private readonly authService = inject(AuthService);

  isCollapsed = false;
  currentYear = new Date().getFullYear();

  // Convert Observable → Signal (Angular 18 best practice)
  currentUser = toSignal(this.authService.currentUser$, { initialValue: null });

  // Example computed value for template
  userName = computed(() =>
    this.currentUser()?.username || this.currentUser()?.email || 'User'
  );

  onUserProfile(): void {
    console.log('User Profile clicked');
  }

  onPasswordReset(): void {
    console.log('Password Reset clicked');
  }

  onLogout(): void {
    this.authService.logout();
  }
}
