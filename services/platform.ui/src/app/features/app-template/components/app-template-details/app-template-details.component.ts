import { Component, OnInit, TemplateRef, ViewChild, inject } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService, NzModalModule } from 'ng-zorro-antd/modal';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { AppTypesharedService } from '../../services/app-template-shared.service';
import { GetAppTypeModel } from '../../../../models/Application-Type/GetAppTypeModel';
import { AppTemplateService } from '../../services/app-template.service';

@Component({
    selector: 'app-app-template-details',
    imports: [ReactiveFormsModule, NzButtonModule, NzFormModule, NzInputModule, NzModalModule, NzTabsModule],
    templateUrl: './app-template-details.component.html',
    styleUrl: './app-template-details.component.css'
})
export class ApplicationTypeDetailsComponent implements OnInit {

  private readonly route = inject(ActivatedRoute);
  private readonly modalService = inject(NzModalService);
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly message = inject(NzMessageService);
  private readonly router = inject(Router);

  private readonly _appTemplateService = inject(AppTemplateService);
  private readonly appTypeSharedService = inject(AppTypesharedService);

  appTypeId!: string;

  validateForm = this.fb.group({
    templateId: ['', Validators.required],
    name: ['', Validators.required],
    description: [''],
    isActive: true
  });

  @ViewChild('approvalFormTemplate', { static: true }) approvalFormTemplate!: TemplateRef<any>;

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const templateId = params.get('templateId');
      if (templateId) {
        this.appTypeId = templateId;
        this.fetchAppTypeDetails(templateId);
      } else {
        this.message.error('Invalid app template ID.');
      }
    });
  }

  private fetchAppTypeDetails(appTypeId: string): void {
    this._appTemplateService.getAppDetails(appTypeId).subscribe({
      next: (data: GetAppTypeModel) => {
        this.validateForm.patchValue(data);
        this.openAppTypeDetailsModal();
      },
      error: (err) => {
        console.error('Error fetching app template:', err);
        this.message.error('Failed to load app template details.');
      }
    });
  }

  updateAppType(): void {
    if (this.validateForm.invalid) {
      this.message.error('Form is invalid');
      return;
    }

    const formData = this.validateForm.getRawValue();

    const updatedAppType: GetAppTypeModel = {
      templateId: formData.templateId,
      name: formData.name,
      description: formData.description || '',
      isActive: formData.isActive
    };

    this.appTypeSharedService.setAppType(updatedAppType);
    this.closeModal();

    this.router.navigate(['/app-template-update']).catch(err => {
      this.message.error(`Navigation error: ${err}`);
    });
  }

  private openAppTypeDetailsModal(): void {
    this.modalService.create({
      nzTitle: 'Template details',
      nzContent: this.approvalFormTemplate,
      nzFooter: [
        {
          label: 'Cancel',
          onClick: () => this.closeModal()
        }
      ],
      nzWidth: 850,
      nzClosable: false,
      nzMaskClosable: true,
      nzOnCancel: () => this.closeModal()
    });    
  }  

  private closeModal(): void {
    this.modalService.closeAll();
    this.router.navigate(['/app-template-list']);
  }
}
