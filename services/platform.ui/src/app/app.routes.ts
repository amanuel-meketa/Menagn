import { Routes } from '@angular/router';
import { UserRegisterComponent } from './features/user/components/user-register/user-register.component';
import { UserListComponent } from './features/user/components/user-list/user-list.component';
import { DashboardComponent } from './features/layout/components/dashboard/dashboard.component';
import { LayoutComponent } from './features/layout/components/layout/layout.component';
import { UserDetailsComponent } from './features/user/components/user-details/user-details.component';
import { UserEditComponent } from './features/user/components/user-edit/user-edit.component';
import { ResetPasswordComponent } from './features/user/components/reset-password/reset-password.component';
import { authGuard } from './guards/auth.guard';
import { UserSessionComponent } from './features/user/components/user-session/user-session.component';
import { UserRoleComponent } from './features/user/components/user-role/user-role.component';
import { RoleListComponent } from './features/role/components/role-list/role-list.component';
import { RoleRegisterComponent } from './features/role/components/role-register/role-register.component';
import { RoleDetailsComponent } from './features/role/components/role-details/role-details.component';
import { RoleUpdateComponent } from './features/role/components/role-update/role-update.component';
import { RoleAssignedUsersComponent } from './features/role/components/role-assigned-users/role-assigned-users.component';
import { AuthCallbackComponent } from './features/auth/components/auth-callback/auth-callback.component';
import { ApplicationTypeListComponent } from './features/application-types/components/application-type-list/application-type-list.component';
import { ApplicationTypeDetailsComponent } from './features/application-types/components/application-type-details/application-type-details.component';
import { ApplicationTypeUpdateComponent } from './features/application-types/components/application-type-update/application-type-update.component';

export const routes: Routes = 
[
  { path: 'auth-callback', component: AuthCallbackComponent, pathMatch: 'full' },
  { path: '', component: LayoutComponent, children: 
    [
      { path: 'dashboard', component: DashboardComponent, canActivate:[authGuard]},
      { path: 'register', component: UserRegisterComponent },
      { path: 'list', component: UserListComponent, canActivate:[authGuard]},
      { path: 'user-details/:id', component: UserDetailsComponent, canActivate:[authGuard]},
      { path: 'user-edit/:id', component: UserEditComponent, canActivate:[authGuard] },
      { path: 'reset-password', component: ResetPasswordComponent, canActivate:[authGuard]},
      { path: 'user-session', component: UserSessionComponent, canActivate:[authGuard]},
      { path: 'user-role', component: UserRoleComponent, canActivate:[authGuard]},
      
      { path: 'role-list', component: RoleListComponent, canActivate:[authGuard]},
      { path: 'role-register', component: RoleRegisterComponent, canActivate:[authGuard]},
      { path: 'role-details/:id', component: RoleDetailsComponent, canActivate:[authGuard]},
      { path: 'role-update', component: RoleUpdateComponent, canActivate:[authGuard]},
      { path: 'role-assigned-users', component: RoleAssignedUsersComponent, canActivate:[authGuard]},
      
      { path: 'app-type-list', component: ApplicationTypeListComponent},
      { path: 'app-type-details/:id', component: ApplicationTypeDetailsComponent},
      { path: 'app-type-update', component: ApplicationTypeUpdateComponent},
    ]
  },
];
