import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

import { NzCardModule } from 'ng-zorro-antd/card';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { NzIconModule } from 'ng-zorro-antd/icon';

import { TemplateHub } from '../../services/template-hub';

@Component({
  standalone: true,
  selector: 'app-template-hub-preview',
  imports: [ CommonModule, NzCardModule, NzTagModule, NzSpinModule, NzStepsModule, NzIconModule],
  templateUrl: './template-hub-preview.html',
  styleUrl: './template-hub-preview.css'
})
export class TemplateHubPreviewComponent implements OnInit {

  private readonly route = inject(ActivatedRoute);
  private readonly hub = inject(TemplateHub);

  loading = true;
  template: any;

  currentStep = 0;
  stepsDirection: 'horizontal' | 'vertical' = 'horizontal';

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
}
