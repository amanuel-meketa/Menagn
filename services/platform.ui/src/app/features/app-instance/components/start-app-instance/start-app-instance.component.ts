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

import { AppInstanceService } from '../../services/app-instance.service';
import { AuthService } from '../../../../shared/services/auth-service.service';
import { ApprovalRequest } from '../../../../models/Approval-Instances/ApprovalRequest';
import { NzBreadCrumbModule } from 'ng-zorro-antd/breadcrumb';

@Component({
  selector: 'start-app-instance',
  standalone: true,
  imports: [ CommonModule, ReactiveFormsModule, NzFormModule, NzCardModule, NzButtonModule, NzDividerModule, NzIconModule,
             NzInputModule, NzBreadCrumbModule ],
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

  // Form with userId included
  validateForm = this.fb.group({
    templateId: [''],
    userId: ['']
  });

  ngOnInit(): void {
    this.templateId = this.route.snapshot.paramMap.get('id');

    // Get logged-in user ID from token
    const userInfo = this.authService.getCurrentUserInfoFromToken();
    const userId = userInfo?.id ?? '';

    if (!this.templateId) {
      this.message.error('No template selected. Redirecting...');
      this.router.navigate(['/app-active-templates']);
      return;
    }

    // Patch templateId and userId
    this.validateForm.patchValue({
      templateId: this.templateId,
      userId
    });
  }

  submitForm(): void {
    if (this.validateForm.invalid) return;

    this.starting = true;
    this.validateForm.disable();

    const newInstance: ApprovalRequest = {
      templateId: this.validateForm.value.templateId!,
      userId: this.validateForm.value.userId!
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
