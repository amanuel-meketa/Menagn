import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { RoleAssignedUsersComponent } from '../../../role/components/role-assigned-users/role-assigned-users.component';
import { ActivatedRoute, Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { ApplicationTypeService } from '../../services/application-type.service';
import { GetAppTypeModel } from '../../../../models/Application-Type/GetAppTypeModel';

@Component({
  selector: 'app-application-type-details',
  standalone: true,
  imports: [ NzButtonModule, NzModalModule, ReactiveFormsModule, NzFormModule, NzInputModule, NzTabsModule, NzButtonModule,
             RoleAssignedUsersComponent],
  templateUrl: './application-type-details.component.html',
  styleUrl: './application-type-details.component.css'
})
export class ApplicationTypeDetailsComponent implements OnInit{
  private route = inject(ActivatedRoute);
  private modal = inject(NzModalService);
  private fb = inject(NonNullableFormBuilder);
  private message = inject(NzMessageService);
  private router = inject(Router);

  private _appTypeService = inject(ApplicationTypeService);

  appTypeId!: string;
  validateForm = this.fb.group({
    id: ['', [Validators.required]], // Ensure this is a required field
    name: ['', [Validators.required]], // Ensure this is a required field
    description: [''] // Optional field
  });
  
  @ViewChild('roleFormTemplate', { static: true }) roleFormTemplate!: TemplateRef<any>;
  
  ngOnInit(): void {
    this.route.paramMap.subscribe(params => { const appTypeId = params.get('id');
      if (appTypeId) {
        this.appTypeId = appTypeId;
        this.fetchAppTypeDetails(appTypeId);
      }
    });
  }

  fetchAppTypeDetails(appTypeId: string): void {
    this._appTypeService.getAppDetails(appTypeId).subscribe({
      next: (role: GetAppTypeModel) => {
        this.validateForm.patchValue(role);
        this.openRoleDetailsModal();
      },
      error: () => {
        this.message.error('Failed to load application type details.');
      }
    });
  }
  updateRole(): void {
  }  

  openRoleDetailsModal(): void {
    this.modal.create({
      nzTitle: 'Role Details',
      nzContent: this.roleFormTemplate,
      nzFooter: [
        {
          label: 'Cancel',
          onClick: () => this.closeModal()
        }
      ],
      nzWidth: 800,
      nzClosable: false,
      nzMaskClosable: false,
      nzOnCancel: () => this.closeModal()
    });
  }

  closeModal(): void {
    console.log('Modal is closing');
    this.modal.closeAll();
    this.router.navigate(['/role-list']);
  }  
  
}

