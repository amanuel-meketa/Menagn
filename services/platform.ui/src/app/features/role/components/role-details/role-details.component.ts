import { Component, inject, TemplateRef, ViewChild, OnInit } from '@angular/core';
import { RoleService } from '../../services/role.service';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { GetRole } from '../../../../models/Role/GetRole';
import { NzTabsModule } from 'ng-zorro-antd/tabs';

@Component({
  selector: 'app-role-details',
  standalone: true,
  imports: [NzButtonModule, NzModalModule, ReactiveFormsModule, NzFormModule, NzInputModule, NzTabsModule],
  templateUrl: './role-details.component.html',
  styleUrl: './role-details.component.css'
})
export class RoleDetailsComponent implements OnInit {

  private _roleService = inject(RoleService);
  private route = inject(ActivatedRoute);
  private modal = inject(NzModalService);
  private fb = inject(NonNullableFormBuilder);
  private message = inject(NzMessageService);
  private router = inject(Router);

  roleId!: string;

  validateForm = this.fb.group({
    id: ['', [Validators.required]],
    name: ['', [Validators.required]],
    description: ['']
  });

  @ViewChild('roleFormTemplate', { static: true }) roleFormTemplate!: TemplateRef<any>;

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const roleId = params.get('id');
      if (roleId) {
        this.roleId = roleId;
        this.fetchRoleDetails(roleId);
      }
    });
  }

  fetchRoleDetails(roleId: string): void {
    this._roleService.getroleDetails(roleId).subscribe({
      next: (role: GetRole) => {
        this.validateForm.patchValue(role);
        this.openRoleDetailsModal();
      },
      error: () => {
        this.message.error('Failed to load role details.');
      }
    });
  }

  openRoleDetailsModal(): void {
    this.modal.create({
      nzTitle: 'Role Details',
      nzContent: this.roleFormTemplate,
      nzFooter: null,
      nzWidth: 800,
      nzClosable: false,
      nzMaskClosable: false,
      nzOnCancel: () => this.closeModal()
    });
  }

  closeModal(): void {
    this.modal.closeAll();
    this.router.navigate(['/role-list']);
  }
}
