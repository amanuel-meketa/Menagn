import { Component, inject, ViewChild, TemplateRef } from '@angular/core';
import { ReactiveFormsModule, NonNullableFormBuilder, Validators } from '@angular/forms';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { RoleService } from '../../services/role.service';
import { CreateRole } from '../../../../models/Role/CreateRole';
import { Router } from '@angular/router';

@Component({
  selector: 'app-role-register',
  standalone: true,
  imports: [NzButtonModule, NzModalModule, ReactiveFormsModule, NzFormModule, NzInputModule],
  templateUrl: './role-register.component.html',
  styleUrls: ['./role-register.component.css']
})
export class RoleRegisterComponent {
  private _roleService = inject(RoleService);
  private modal = inject(NzModalService);
  private fb = inject(NonNullableFormBuilder);
  private message = inject(NzMessageService);
  private rout = inject(Router);

  validateForm = this.fb.group({
    name: ['', [Validators.required]],
    description: ['']
  });

  @ViewChild('roleFormTemplate', { static: true }) roleFormTemplate!: TemplateRef<any>;

  openRoleRegisterModal(): void {
    this.modal.create({
      nzTitle: 'Register Role',
      nzContent: this.roleFormTemplate,
      nzFooter: null,
      nzWidth: 600
    });
  }

  cancel(): void {
    this.modal.closeAll();
  }

  submitForm(): void {
    if (this.validateForm.invalid) {
      return; 
    }

    const roleData: CreateRole = this.prepareRoleData();

    this.createRole(roleData);
  }

  private prepareRoleData(): CreateRole {
    return {
      name: this.validateForm.value.name?.trim() || '',
      description: this.validateForm.value.description?.trim() || '' 
    };
  }

  private createRole(roleData: CreateRole): void {
    this._roleService.createRole(roleData).subscribe({
      next: (response) => this.handleSuccess(response),
      error: (error) => this.handleError(error)
    });
  }

  private handleSuccess(response: any): void {
    this.message.remove();
    this.message.success(`Role "${response.name}" created successfully!`);
    console.log('Role created successfully:', response);
    this.cancel();

    this.rout.navigate(['/role-list']);
  }

  private handleError(error: any): void {
    this.message.remove();
    const errorMessage = error?.message || 'Unknown error occurred';
    this.message.error(`Registration failed: ${errorMessage}`);
    console.error('Role creation failed:', error);
  }
}
