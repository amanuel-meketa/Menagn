import { Component, inject, OnInit } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzBreadCrumbModule } from 'ng-zorro-antd/breadcrumb';

import { AppInstanceService } from '../../services/app-instance.service';
import { AuthService } from '../../../../shared/services/auth-service.service';
import { ApprovalRequest } from '../../../../models/Approval-Instances/ApprovalRequest';
import { NzStepsModule } from 'ng-zorro-antd/steps';

@Component({
  selector: 'start-app-instance',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzFormModule,
    NzCardModule,
    NzButtonModule,
    NzDividerModule,
    NzIconModule,
    NzInputModule,
    NzBreadCrumbModule,
    NzStepsModule ],
  templateUrl: './start-app-instance.component.html',
  styleUrls: ['./start-app-instance.component.css']
})
export class StartAppInstanceComponent implements OnInit {
  private fb = inject(NonNullableFormBuilder);
  private message = inject(NzMessageService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private appInstanceService = inject(AppInstanceService);
  private authService = inject(AuthService);

  templateId!: string | null;
  starting = false;

  // Form only contains templateId
  validateForm = this.fb.group({
    templateId: ['']
  });

  ngOnInit(): void {
    this.templateId = this.route.snapshot.paramMap.get('id');

    if (!this.templateId) {
      this.message.error('No template selected. Redirecting...');
      this.router.navigate(['/app-active-templates']);
      return;
    }

    this.validateForm.patchValue({ templateId: this.templateId });
  }

  get currentUserDisplay(): string {
      return this.authService.currentUser?.username || this.authService.currentUser?.email || '';
  }

  submitForm(): void {
    if (this.validateForm.invalid) return;

    const currentUser = this.authService.currentUser;
    if (!currentUser) {
      this.message.error('User not loaded. Please refresh.');
      return;
    }

    this.starting = true;
    this.validateForm.disable();

    const newInstance: ApprovalRequest = {
      templateId: this.validateForm.value.templateId!,
      userId: currentUser.userId || currentUser.userId
    };

    this.appInstanceService.startApprovalInstance(newInstance).subscribe({
      next: () => {
        this.message.success('Approval instance started successfully!');
        this.router.navigate(['/app-active-templates']);
      },
      error: (err) => {
        this.message.error(err?.error?.message || 'Failed to start instance');
        this.validateForm.enable();
      },
      complete: () => (this.starting = false)
    });
  }

  goBack(): void {
    this.router.navigate(['/app-active-templates']);
  }
}
