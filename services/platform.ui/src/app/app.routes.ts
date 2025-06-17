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
import { ApplicationTypeDetailsComponent } from './features/app-template/components/app-template-details/app-template-details.component';
import { ApplicationTypeUpdateComponent } from './features/app-template/components/app-template-update/app-template-update.component';
import { AppTemplateListComponent } from './features/app-template/components/app-template-list/app-template-list.component';
import { AppTemplateCreateComponent } from './features/app-template/components/app-template-create/app-template-create.component';
import { StageDefinitionListComponent } from './features/stage-definition/components/stage-definition-list/stage-definition-list.component';
import { StageDefinitionDetailsComponent } from './features/stage-definition/components/stage-definition-details/stage-definition-details.component';
import { StageDefinitionUpdateComponent } from './features/stage-definition/components/stage-definition-update/stage-definition-update.component';
import { StartAppInstanceComponent } from './features/app-instance/components/start-app-instance/start-app-instance.component';

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
      
      { path: 'app-template-list', component: AppTemplateListComponent},
      { path: 'app-template-create', component: AppTemplateCreateComponent},
      { path: 'app-template-details/:templateId', component: ApplicationTypeDetailsComponent},
      { path: 'app-template-update', component: ApplicationTypeUpdateComponent},

      { path: 'stage-definition-list', component: StageDefinitionListComponent},
      { path: 'stage-definition-details/:key', component: StageDefinitionDetailsComponent},
      { path: 'stage-definition-update', component: StageDefinitionUpdateComponent},
      
      { path: 'start-instance/:templateId', component: StartAppInstanceComponent}
     
    ]
  },
];
