import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';

import { NzCardModule } from 'ng-zorro-antd/card';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';

import { TemplateHub } from '../../services/template-hub';
import { AppTemplateService } from '../../../app-template/services/app-template.service';
import { StageDefinitionService } from '../../../stage-definition/services/stage-definition.service';
import { CreateAppTemplateModel } from '../../../../models/Application-Type/CreateAppTemplateModel';
import { AddStageDefiModel } from '../../../../models/Stage-Definition/AddStageDefiModel';
import { HttpResponse } from '@angular/common/http';

@Component({
  standalone: true,
  selector: 'app-template-hub-preview',
  imports: [ CommonModule, NzCardModule, NzTagModule, NzSpinModule, NzStepsModule, NzIconModule, NzButtonModule ],
  templateUrl: './template-hub-preview.html',
  styleUrl: './template-hub-preview.css'
})
export class TemplateHubPreviewComponent implements OnInit {

  private readonly route = inject(ActivatedRoute);
  private readonly hub = inject(TemplateHub);
  private readonly templateService = inject(AppTemplateService);
  private readonly stageService = inject(StageDefinitionService);
  private readonly message = inject(NzMessageService);

  loading = true;
  template: any;

  currentStep = 0;
  stepsDirection: 'horizontal' | 'vertical' = 'horizontal';

  // track install button state
  installing = false;

  ngOnInit(): void {
    const rawKey = this.route.snapshot.paramMap.get('key');
    if (!rawKey) return;

    const key = decodeURIComponent(rawKey);
    this.load(key);
  }

  load(key: string): void {
    this.loading = true;

    this.hub.getTemplateDetails(key).subscribe({
      next: res => {
        this.template = res;

        // sort stages
        this.template.stageDefinitions = this.template.stageDefinitions?.sort(
            (a: any, b: any) => a.sequenceOrder - b.sequenceOrder);

        // auto vertical for long workflows
        this.stepsDirection = this.template.stageDefinitions?.length > 4? 'vertical' : 'horizontal';

        this.currentStep = 0;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  /** Icon mapping: User vs Role */
  getStepIcon(type?: string): string {
    switch (type) {
      case 'User':
        return 'user';
      case 'Role':
        return 'team';
      default:
        return 'solution';
    }
  }

  /** INSTALL TEMPLATE + STAGES */
installTemplate(): void {
  if (this.installing || !this.template) return;

  this.installing = true;

  const loadingMsgId = this.message.loading(
    'Installing template and workflow stages...',
    { nzDuration: 0 }
  ).messageId;

  const payload: CreateAppTemplateModel = {
    name: this.template.name,
    description: this.template.description,
    isActive: true
  };

  this.templateService.createAppTemplate(payload).subscribe({
    next: (response) => {
      const templateId = JSON.parse(response.body as string);

      if (!templateId) {
        this.message.remove(loadingMsgId);
        this.message.error('Template installation failed. Please try again.');
        this.installing = false;
        return;
      }

      const stages: AddStageDefiModel[] =
        this.template.stageDefinitions.map((s: any, i: number) => ({
          templateId,
          stageName: s.stageName,
          description: s.description,
          sequenceOrder: s.sequenceOrder ?? i + 1,
          assignmentType: s.assignmentType,
          assignmentKey: s.assignmentKey
        }));

      this.stageService.addStages(stages).subscribe({
        next: () => {
          this.message.remove(loadingMsgId);
          this.message.success(
            `Template "${this.template.name}" was installed successfully with ${stages.length} stages.`
          );
          this.installing = false;
        },
        error: () => {
          this.message.remove(loadingMsgId);
          this.message.error(
            'Template was created, but workflow stage installation failed. Please contact support.'
          );
          this.installing = false;
        }
      });
    },
    error: () => {
      this.message.remove(loadingMsgId);
      this.message.error(
        'Template installation failed. No changes were applied.'
      );
      this.installing = false;
    }
  });
}
}