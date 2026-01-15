import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzTimelineModule } from 'ng-zorro-antd/timeline';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzSpinModule } from 'ng-zorro-antd/spin';

import { TemplateHub } from '../../services/template-hub';
import { NzStepsModule } from 'ng-zorro-antd/steps';

@Component({
  standalone: true,
  selector: 'app-template-hub-preview',
  imports: [
    CommonModule,
    NzCardModule,
    NzTimelineModule,
    NzTagModule,
    NzSpinModule,
     CommonModule,
  NzStepsModule
  ],
  templateUrl: './template-hub-preview.html',
  styleUrl: './template-hub-preview.css'
})
export class TemplateHubPreviewComponent implements OnInit {

  private readonly route = inject(ActivatedRoute);
  private readonly hub = inject(TemplateHub);

  loading = true;
  template: any;
  currentStep = 0;

  ngOnInit(): void {
    const key = this.route.snapshot.paramMap.get('key')!;
    this.load(key);
  }

  load(key: string): void {
    this.loading = true;

    this.hub.getTemplateDetails(key).subscribe({
      next: res => {
        this.template = res;
        this.template.stageDefinitions = this.template.stageDefinitions?.sort((a: any, b: any) => a.sequenceOrder - b.sequenceOrder);

        this.loading = false;
      },
      error: () => this.loading = false
    });
  }
}
