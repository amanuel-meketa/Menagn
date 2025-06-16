import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { RoleAssignedUsersComponent } from '../../../role/components/role-assigned-users/role-assigned-users.component';
import { ActivatedRoute, Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { GetStageDefiModel } from '../../../../models/Stage-Definition/GetStageDefiModel';
import { StageDefinitionService } from '../../services/stage-definition.service';

@Component({
  selector: 'app-stage-definition-details',
  standalone: true,
  imports: [ ReactiveFormsModule, NzButtonModule, NzFormModule, NzInputModule, NzModalModule, NzTabsModule, RoleAssignedUsersComponent ],
  templateUrl: './stage-definition-details.component.html',
  styleUrl: './stage-definition-details.component.css'
})

export class StageDefinitionDetailsComponent  implements OnInit {

  private readonly route = inject(ActivatedRoute);
  private readonly modalService = inject(NzModalService);
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly message = inject(NzMessageService);
  private readonly router = inject(Router);

  private readonly _stageDefiService = inject(StageDefinitionService);
  stageDefinitionId!: any;

  validateForm = this.fb.group({
    stageDefId: ['', Validators.required],
    stageName: ['', Validators.required],
    description: ['']
  });

  @ViewChild('stageFormTemplate', { static: true }) stageFormTemplate!: TemplateRef<any>;

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const stageId = params.get('key');
      if (stageId) {
        this.stageDefinitionId = stageId;
        this.fetchStageDetails(stageId);
      } else {
        this.message.error('Invalid stage Id.');
      }
    });
  }

  private fetchStageDetails(stageDefinitionId: any): void {
    this._stageDefiService.getStageDefiDetails(stageDefinitionId).subscribe({ 
      next: (data: GetStageDefiModel) => {
        console.log(data)
        this.validateForm.patchValue(data);
        this.openAppTypeDetailsModal();
      },
      error: (err: any) => { 
        console.error('Error fetching app template:', err);
        this.message.error('Failed to load app template details.');
      }
    });
  }
    
  updateAppType(): void {
  }

  private openAppTypeDetailsModal(): void {
    this.modalService.create({
      nzTitle: 'Stage details',
      nzContent: this.stageFormTemplate,
      nzFooter: [
        {
          label: 'Cancel',
          onClick: () => this.closeModal()
        }
      ],
      nzWidth: 700,
      nzClosable: false,
      nzMaskClosable: true,
      nzOnCancel: () => this.closeModal()
    });
  }

  private closeModal(): void {
    this.modalService.closeAll();
    this.router.navigate(['/stage-definition-list']);
  }

}
