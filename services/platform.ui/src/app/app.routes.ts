import { Routes } from '@angular/router';
import { LoginComponent } from './features/user/components/login/login.component';
import { UserRegisterComponent } from './features/user/components/user-register/user-register.component';
import { UserListComponent } from './features/user/components/user-list/user-list.component';
import { DashboardComponent } from './features/layout/components/dashboard/dashboard.component';
import { LayoutComponent } from './features/layout/components/layout/layout.component';
import { UserDetailsComponent } from './features/user/components/user-details/user-details.component';
import { UserEditComponent } from './features/user/components/user-edit/user-edit.component';
import { ResetPasswordComponent } from './features/user/components/reset-password/reset-password.component';

export const routes: Routes = 
[
  { path: 'login', component: LoginComponent },
  { path: '', component: LayoutComponent,
    children: 
    [
      { path: 'dashboard', component: DashboardComponent },
      { path: 'register', component: UserRegisterComponent },
      { path: 'list', component: UserListComponent },
      { path: 'user-details/:id', component: UserDetailsComponent },
      { path: 'user-edit/:id', component: UserEditComponent },
      { path: 'reset-password', component: ResetPasswordComponent}
    ]
   },
];
