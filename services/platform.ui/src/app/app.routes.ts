import { Routes } from '@angular/router';
import { LoginComponent } from './features/user/components/login/login.component';
import { UserRegisterComponent } from './features/user/components/user-register/user-register.component';
import { UserListComponent } from './features/user/components/user-list/user-list.component';
import { DashboardComponent } from './features/layout/components/dashboard/dashboard.component';
import { LayoutComponent } from './features/layout/components/layout/layout.component';

export const routes: Routes = 
[
  { path: 'login', component: LoginComponent },
  { path: '', component: LayoutComponent,
    children: 
    [
      { path: 'dashboard', component: DashboardComponent},
      { path: 'register', component: UserRegisterComponent },
      { path: 'list', component: UserListComponent },
    ]
   },
];
