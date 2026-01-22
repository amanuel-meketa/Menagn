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
import { CreatedBy } from '../../../../models/User/CreatedBy';
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzModalService } from 'ng-zorro-antd/modal';
import { StageDefinitionService } from '../../../stage-definition/services/stage-definition.service';
import { GetStageDefiModel } from '../../../../models/Stage-Definition/GetStageDefiModel';

@Component({
  selector: 'start-app-instance',
  imports: [ CommonModule, ReactiveFormsModule, NzFormModule, NzCardModule, NzStepsModule,
             NzButtonModule, NzDividerModule, NzIconModule, NzInputModule, NzBreadCrumbModule,
     NzModalModule ],
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
  private modal = inject(NzModalService);
  private stageService = inject(StageDefinitionService);

  // optional template details
  templateDescription: string | null = null;
  totalInstances?: number;
  activeInstances?: number;
  stages: GetStageDefiModel[] = [];
  currentStep = 0;

  templateId!: string | null;
  templateName!: string | null;
  starting = false;

  // Form only contains templateId
  validateForm = this.fb.group({
    templateId: ['']
  });

  ngOnInit(): void {
    this.templateId = this.route.snapshot.paramMap.get('id');
    this.templateName = this.route.snapshot.queryParamMap.get('name');

    if (!this.templateId) {
      this.message.error('No template selected. Redirecting...');
      this.router.navigate(['/app-active-templates']);
      return;
    }

    this.validateForm.patchValue({ templateId: this.templateId });
    // load workflow stages for this template so steps show real stages
    if (this.templateId) {
      this.stageService.getStagesByTempId(this.templateId).subscribe({
        next: (s) => {
          this.stages = (s || []).slice().sort((a: any, b: any) => (a.sequenceOrder || 0) - (b.sequenceOrder || 0));
          // default to first step
          this.currentStep = 0;
        },
        error: () => {
          this.stages = [];
        }
      });
    }
  }

  get currentUserDisplay(): string {
      return this.authService.currentUser?.username || this.authService.currentUser?.email || '';
  }

 submitForm(): void {
  if (this.validateForm.invalid || !this.templateId) return;

  const currentUser = this.authService.currentUser;
  if (!currentUser) {
    this.message.error('User not loaded. Please refresh.');
    return;
  }

  const userInfo: CreatedBy = {
    userId: currentUser.userId,
    fullName: currentUser.fullName || ''
  };

  this.modal.confirm({
    nzTitle: 'Start approval instance',
    nzContent: `Start an approval instance from template "${this.templateName || this.templateId}" as ${this.currentUserDisplay}?`,
    nzOkText: 'Start',
    nzCancelText: 'Cancel',
    nzOnOk: () => {
      this.starting = true;
      this.validateForm.disable();
      return new Promise<void>((resolve, reject) => {
        this.appInstanceService.startApprovalInstance(this.templateId!, userInfo).subscribe({
          next: () => {
            this.message.success('Approval instance started successfully!');
            this.router.navigate(['/app-active-templates']);
            resolve();
          },
          error: (err) => {
            this.message.error(err?.error?.message || 'Failed to start instance');
            this.validateForm.enable();
            this.starting = false;
            reject(err);
          }
        });
      });
    }
  });
}


  goBack(): void {
    this.router.navigate(['/app-active-templates']);
  }
}
