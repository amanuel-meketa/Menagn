import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { GetRole } from '../../../../models/Role/GetRole';
import { RoleService } from '../../services/role.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { RolesharedService } from '../../services/Roleshared.service';

@Component({
    selector: 'app-role-update',
    templateUrl: './role-update.component.html',
    styleUrls: ['./role-update.component.css'],
    standalone: false
})
export class RoleUpdateComponent implements OnInit {
  private _roleService = inject(RoleService);
  private _rolesharedService = inject(RolesharedService);
  roleData: GetRole | null = null;
  private message = inject(NzMessageService);
  private router = inject(Router);

  ngOnInit(): void {
    this.subscribeToRoleData();
  }

  private subscribeToRoleData(): void {
    this._rolesharedService.currentRole$.subscribe({
      next: (role) => {
        if (role) {
          this.roleData = role;
          this.updateRole();
        } else {
          console.log('No role data received from shared service');
        }
      },
      error: (err) => {
        this.message.error('Error subscribing to role data:', err);
      }
    });
  }

  private updateRole(): void {
    if (!this.roleData) {
      this.message.error('Role data is missing or invalid');
      return;
    }

    const updatedRole = {
      name: this.roleData.name || '',
      description: this.roleData.description || ''
    };

    this._roleService.updateRole(this.roleData.id, updatedRole).subscribe({
      next: (response) => {
        this.message.success('Role updated successfully');
        this.router.navigate(['/role-list']);
        
      },
      error: (err) => {
        const errorMessage = err?.error?.message || 'Failed to update role';
        this.message.error(errorMessage);
      }
    });
  }
}
