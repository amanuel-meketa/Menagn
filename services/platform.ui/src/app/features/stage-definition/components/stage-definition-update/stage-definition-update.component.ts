import { Component, inject, OnInit } from '@angular/core';
import { StageDefinitionService } from '../../services/stage-definition.service';
import { Router } from '@angular/router';
import { StageDefinitionSharedService } from '../../services/stage-definition-shared.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { UpdateStageDefiModel } from '../../../../models/Stage-Definition/UpdateStageDefiModel';
import { GetStageDefiModel } from '../../../../models/Stage-Definition/GetStageDefiModel';

@Component({
    selector: 'app-stage-definition-update',
    imports: [],
    templateUrl: './stage-definition-update.component.html',
    styleUrl: './stage-definition-update.component.css'
})
export class StageDefinitionUpdateComponent implements OnInit {

  private readonly _stageDefiService = inject(StageDefinitionService);
  private readonly _stageDefiSharedService = inject(StageDefinitionSharedService);
  private readonly messageService = inject(NzMessageService);
  private readonly router = inject(Router);
  
  stageData: UpdateStageDefiModel | null = null;

  ngOnInit(): void {
    this.loadStageData();
  }

  private loadStageData(): void {
    this._stageDefiSharedService.currentstageDefin$.subscribe({
      next: (data: GetStageDefiModel | null) => {
        if (!data) {
          this.messageService.error('Received null stage data');
          return;
        }

        this.stageData = {
          id: data.stageDefId,
          name: data.stageName,
          description: data.description
        };

        this.submitStageUpdate();
      },
      error: (error: any) => {
        console.error('Failed to retrieve stage data:', error);
        this.messageService.error('Error subscribing to stage data');
      }
    });
  }

  private submitStageUpdate(): void {
    if (!this.stageData) {
      this.messageService.error('Stage data is missing or invalid');
      return;
    }

    const { id, name = '', description = '' } = this.stageData;

    const updatedStage: UpdateStageDefiModel = { id, name, description };

    this._stageDefiService.updateStageDefi(id, updatedStage).subscribe({
      next: () => {
        this.messageService.success('Stage updated successfully');
        this.router.navigate(['/stage-definition-list']);
      },
      error: (error: any) => {
        console.error('Error updating stage:', error);
        const errorMessage = error?.error?.message || error?.message || 'Failed to update stage';
        this.messageService.error(errorMessage);
      }
    });
  }
}