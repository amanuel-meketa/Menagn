import { Component, inject, Input, TemplateRef, ViewChild } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { AppInstanceService } from '../../services/app-instance.service';
import { ApprovalRequest } from '../../../../models/Approval-Instances/ApprovalRequest';

@Component({
  selector: 'app-start-app-instance',
  standalone: true,
  imports: [ NzButtonModule, NzModalModule, ReactiveFormsModule, NzFormModule, NzInputModule ],
  templateUrl: './start-app-instance.component.html',
  styleUrl: './start-app-instance.component.css'
})
export class StartAppInstanceComponent {

  @Input() templateId!: string;
  private fb = inject(NonNullableFormBuilder);
  private modal = inject(NzModalService);
  private message = inject(NzMessageService);
  private router = inject(Router);
  private _appInstanceService = inject(AppInstanceService);

  @ViewChild('startApprovalTemplate', { static: true }) startApprovalTemplate!: TemplateRef<any>;

  /** Open the modal and keep reference to it */
  openTemplateRegisterModal(): void {
    // Set the templateId value to the form before showing the modal
    this.validateForm.patchValue({
      templateId: this.templateId
    });
  
    this.modal.create({
      nzTitle: 'Use Template',
      nzContent: this.startApprovalTemplate,
      nzFooter: null,
      nzWidth: 800,
      nzMaskClosable: false
    });
  }
  

  /** Submit form */
  submitForm(): void {
    if (this.validateForm.invalid) return;

    const newInstance: ApprovalRequest = {
      templateId: this.validateForm.value.templateId?.trim() || '',
      userId: this.validateForm.value.userId?.trim() || ''
    };

    this._appInstanceService.startApprovalInstance(newInstance).subscribe({
      next: (response) => this.onCreateSuccess(response),
      error: (error) => this.onCreateError(error)
    });
  }

  /** Handle successful creation */
  private onCreateSuccess(response: any): void {
    this.message.remove();
    this.message.success(`Instance started successfully!`);
    this.cancel();

    // Optionally navigate
    this.router.navigate(['/app-template-list']);
  }

  /** Handle error */
  private onCreateError(error: any): void {
    this.message.remove();
    const errMsg = error?.error?.message || error?.message || 'Unknown error occurred';
    this.message.error(`Creation failed: ${errMsg}`);
    console.error('instance creation failed:', error);
  }

  cancel(): void {
    this.modal.closeAll();
  }

  validateForm = this.fb.group({
    templateId: [''],
    userId: ['']
  });
}
