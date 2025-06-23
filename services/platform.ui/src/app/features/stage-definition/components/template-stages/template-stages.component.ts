import { Component, inject, Input } from '@angular/core';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { GetStageDefiModel } from '../../../../models/Stage-Definition/GetStageDefiModel';
import { StageDefinitionService } from '../../services/stage-definition.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-template-stages',
  standalone: true,
  imports: [ NzButtonModule, NzModalModule, NzDividerModule, NzStepsModule, NzDescriptionsModule, NzTagModule, CommonModule ],
  templateUrl: './template-stages.component.html',
  styleUrl: './template-stages.component.css'
})
export class TemplateStagesComponent {
  private readonly _stageDefiService = inject(StageDefinitionService);

  @Input() templateId: string | null = null;

  isVisible = false;
  index = 0;
  stages: GetStageDefiModel[] = [];

  openModal(): void {
    if (!this.templateId) {
      console.warn('Template ID is not set!');
      return;
    }

    this._stageDefiService.getStagesByTempId(this.templateId).subscribe({
      next: (data) => {
        console.log('Stages received:', data);
        this.stages = data;
        this.index = 0;
        this.isVisible = true;
      },
      error: (err) => {
        console.error('Failed to fetch stages', err);
        this.stages = [];
        this.isVisible = false;
      }
    });
  }

  handleCancel(): void {
    this.isVisible = false;
  }

  onIndexChange(index: number): void {
    this.index = index;
  }

  get selectedStage(): GetStageDefiModel {
    return this.stages[this.index] || {
      stageName: '',
      description: '',
      assignmentType: '',
      assignmentKey: '',
      sequenceOrder: 0,
      stageDefId: '',
      templateId: '',
    };
  }

  getStatus(): string {
    return 'Active'; // customize as needed
  }
  // Example methods for status text and color per stage
getStatusForStage(stage: any): string {
  return stage.status || 'Unknown';
}

getStatusColorForStage(stage: any): string {
  switch (stage.status) {
    case 'Completed':
      return 'green';
    case 'In Progress':
      return 'blue';
    case 'Pending':
      return 'orange';
    case 'Failed':
      return 'red';
    default:
      return 'default';
  }
}

  getStatusColor(): string {
    return 'blue';
  }
}
