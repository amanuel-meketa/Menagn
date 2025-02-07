import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzBreadCrumbModule } from 'ng-zorro-antd/breadcrumb';
import { RouterLink, RouterOutlet } from '@angular/router';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';


@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, CommonModule, NzBreadCrumbModule, NzIconModule, NzLayoutModule,
            NzMenuModule, NzButtonModule, NzDropDownModule],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css'
})
export class LayoutComponent {
isCollapsed = false;
  currentYear: number = new Date().getFullYear();

  onPasswordReset(): void {
    // Implement password reset logic here
    console.log('Password Reset clicked');
  }

  onUserProfile(): void {
    // Implement user profile logic here
    console.log('User Profile clicked');
  }

  onLogout(): void {
    // Implement logout logic here
    console.log('Logout clicked');
  }
}
