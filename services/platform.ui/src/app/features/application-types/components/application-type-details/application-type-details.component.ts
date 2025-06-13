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
import { UpdateAppTypeMode } from '../../../../models/Application-Type/UpdateAppTypeMode';
import { AppTypesharedService } from '../../services/application-type-shared.service';

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
  private _appTypeSharedService = inject(AppTypesharedService);

  appTypeId!: string;
  validateForm = this.fb.group({
    templateId: ['', [Validators.required]], 
    name: ['', [Validators.required]],
    description: ['']
  });
  
  @ViewChild('roleFormTemplate', { static: true }) roleFormTemplate!: TemplateRef<any>;
  
  ngOnInit(): void {
    this.route.paramMap.subscribe(params => { const appTypeId = params.get('templateId');
      if (appTypeId) {
        this.appTypeId = appTypeId;
        console.log("custom log" + appTypeId);
        this.fetchAppTypeDetails(appTypeId);
      }
    });
  }

  fetchAppTypeDetails(appTypeId: any): void {
    this._appTypeService.getAppDetails(appTypeId).subscribe({
      next: (role: GetAppTypeModel) => {
        this.validateForm.patchValue(role);
        this.openAppTypeDetailsModal();
      },
      error: () => {
        this.message.error('Failed to load application type details.');
      }
    });
  }

  updateAppType(): void {
    if (this.validateForm.valid) {
      const appTypeData = this.validateForm.value;
  
      const updatedRole: UpdateAppTypeMode = {
        templateId: appTypeData.templateId || '', 
        name: appTypeData.name || '',
        description: appTypeData.description || ''
      };
  
      this._appTypeSharedService.setAppType(updatedRole);
  
      this.closeModal();
  
      this.router.navigate(['/app-type-update']).then(() => {
        console.log('Navigation complete');
      }).catch((err) => {
        this.message.error('Navigation error: ' + err);
      });
    } else {
      this.message.error('Form is invalid');
    }
  }  
  
  openAppTypeDetailsModal(): void {
    this.modal.create({
      nzTitle: 'Applicatio Type Details',
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
    this.modal.closeAll();
    this.router.navigate(['/app-type-list']);
  }  
  
}
