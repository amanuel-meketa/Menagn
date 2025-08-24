import { Component, inject, OnInit } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';

// NG-Zorro Modules
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzMessageService } from 'ng-zorro-antd/message';

// Services & Models
import { AppInstanceService } from '../../services/app-instance.service';
import { AuthService } from '../../../../shared/services/auth-service.service';
import { ApprovalRequest } from '../../../../models/Approval-Instances/ApprovalRequest';

@Component({
  selector: 'start-app-instance',
  standalone: true,
  imports: [ CommonModule, ReactiveFormsModule, NzFormModule, NzCardModule, NzButtonModule, NzDividerModule, NzIconModule,
             NzInputModule],
  templateUrl: './start-app-instance.component.html',
  styleUrls: ['./start-app-instance.component.css']
})
export class StartAppInstanceComponent implements OnInit {
  // Injected Services
  private fb = inject(NonNullableFormBuilder);
  private message = inject(NzMessageService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private appInstanceService = inject(AppInstanceService);
  private authService = inject(AuthService);

  // Component State
  validateForm = this.fb.group({
    templateId: [''],
    userId: ['', [Validators.required]]
  });
  templateId!: string | null;
  starting = false;
  loadingUser = true;

  ngOnInit(): void {
    // Get templateId from route params
    this.templateId = this.route.snapshot.paramMap.get('id');

    if (!this.templateId) {
      this.message.error('No template selected. Redirecting...');
      this.router.navigate(['/app-active-templates']);
      return;
    }

    // Set templateId in form
    this.validateForm.patchValue({ templateId: this.templateId });

    // Auto-fill current user
    this.authService.getCurrentUser().subscribe({
      next: (user) => {
        if (user) {
          this.validateForm.patchValue({ userId: user.userId });
        }
      },
      complete: () => (this.loadingUser = false)
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
        this.message.success(' Approval instance started successfully!');
        this.router.navigate(['/app-template-list']);
      },
      error: (err) => {
        this.message.error(err?.error?.message || 'Failed to start instance');
        this.validateForm.enable();
      },
      complete: () => (this.starting = false)
    });
  }
}
