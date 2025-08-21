import { Component, inject, Input, TemplateRef, ViewChild, OnInit } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { AppInstanceService } from '../../services/app-instance.service';
import { ApprovalRequest } from '../../../../models/Approval-Instances/ApprovalRequest';
import { AuthService } from '../../../../shared/services/auth-service.service';

@Component({
  selector: 'app-start-app-instance',
  standalone: true,
  imports: [NzButtonModule, ReactiveFormsModule, NzFormModule],
  templateUrl: './start-app-instance.component.html',
  styleUrls: ['./start-app-instance.component.css']
})
export class StartAppInstanceComponent implements OnInit {
  @Input() templateId!: string;

  private fb = inject(NonNullableFormBuilder);
  private modal = inject(NzModalService);
  private message = inject(NzMessageService);
  private router = inject(Router);
  private _appInstanceService = inject(AppInstanceService);
  private _authService = inject(AuthService);

  @ViewChild('startApprovalTemplate', { static: true })
  startApprovalTemplate!: TemplateRef<any>;

  validateForm = this.fb.group({
    templateId: [''],
    userId: ['', [Validators.required]]
  });

  currentUserId: string = '';

  ngOnInit(): void {
    // Get the current user ID
    this._authService.currentUser$.subscribe(user => {
      if (user) {
        this.currentUserId = user.nameIdentifier; // assuming 'id' is the user ID property
      }
    });
  }

  /** Open modal and auto-fill templateId and current userId */
  openTemplateRegisterModal(): void {
    // Get the current logged-in user ID
    this._authService.getCurrentUser().subscribe(user => {
      this.validateForm.patchValue({ 
        templateId: this.templateId,
        userId: user.nameIdentifier // assuming your GetCurrentUser model has `userId`
      });
  
      this.modal.create({
        nzTitle: 'Start Template Instance',
        nzContent: this.startApprovalTemplate,
        nzFooter: null,
        nzWidth: 600,
        nzMaskClosable: false
      });
    });
  }
  

  /** Submit the start instance form */
  submitForm(): void {
    if (this.validateForm.invalid) return;

    const newInstance: ApprovalRequest = {
      templateId: this.validateForm.value.templateId!.trim(),
      userId: this.validateForm.value.userId!.trim()
    };

    this._appInstanceService.startApprovalInstance(newInstance).subscribe({
      next: () => {
        this.message.remove();
        this.message.success('Instance started successfully!');
        this.modal.closeAll();
        this.router.navigate(['/app-template-list']);
      },
      error: (err) => {
        this.message.remove();
        this.message.error(err?.error?.message || 'Failed to start instance');
        console.error('Start instance error:', err);
      }
    });
  }

  cancel(): void {
    this.modal.closeAll();
  }
}
