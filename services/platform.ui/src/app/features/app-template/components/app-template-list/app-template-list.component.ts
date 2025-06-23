import { ChangeDetectorRef, Component, inject, OnInit, OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { GetAppTypeModel } from '../../../../models/Application-Type/GetAppTypeModel';
import { CustomColumn } from '../../../../shared/model/custom-column';
import { Subject, takeUntil } from 'rxjs';
import { AppTemplateService } from '../../services/app-template.service';
import { AppTemplateCreateComponent } from '../app-template-create/app-template-create.component';
import { NzCardModule } from 'ng-zorro-antd/card';
import { CommonModule } from '@angular/common';
import { AppInstanceService } from '../../../app-instance/services/app-instance.service';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { TemplateStagesComponent } from '../../../stage-definition/components/template-stages/template-stages.component';

@Component({
  selector: 'app-app-template-list',
  standalone: true,
  imports: [ NzButtonModule, NzGridModule, NzIconModule, NzModalModule, NzCardModule, CommonModule, RouterModule, 
             AppTemplateCreateComponent, NzTagModule, TemplateStagesComponent ],
  templateUrl: './app-template-list.component.html',
  styleUrl: './app-template-list.component.css'
})

export class AppTemplateListComponent implements OnInit, OnDestroy {
  private readonly _appTemplateService = inject(AppTemplateService);
  private readonly _appInstanceeService = inject(AppInstanceService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly message = inject(NzMessageService);
  private readonly destroy$ = new Subject<void>();
  private readonly model = inject(NzModalService);
  listOfData: GetAppTypeModel[] = [];
 
  customColumn: CustomColumn[] = [
    { name: 'Name', value: 'name', default: true, required: true, position: 'left', width: 100, fixWidth: true },
    { name: 'Description', value: 'description', default: true, width: 400 },
    { name: 'Action', value: 'action', default: true, required: true, position: 'right', width: 50 }
  ];

  isVisible = false;
  title: CustomColumn[] = [];
  footer: CustomColumn[] = [];
  fix: CustomColumn[] = [];
  notFix: CustomColumn[] = [];

  ngOnInit(): void {
    this.loadAppTypeList();

    this.title = this.customColumn.filter(item => item.position === 'left' && item.required);
    this.footer = this.customColumn.filter(item => item.position === 'right' && item.required);
    this.fix = this.customColumn.filter(item => item.default && !item.required);
    this.notFix = this.customColumn.filter(item => !item.default && !item.required);

    this._appTemplateService.AppTypeListUpdated$.pipe(takeUntil(this.destroy$)).subscribe(
      () => this.loadAppTypeList());
   }

   private loadAppTypeList(): void {
    this._appTemplateService.getAppTemplateList().subscribe({
      next: (data: GetAppTypeModel[]) => {
        this.listOfData = data;
  
        data.forEach(template => {
          this._appInstanceeService.getInstanceByTempId(template.templateId).subscribe(instances => {
            const total = instances.length;
            const active = instances.filter(i => i.overallStatus === 'Pending').length;
  
            // Update the corresponding template in listOfData
            const item = this.listOfData.find(t => t.templateId === template.templateId);
            if (item) {
              item.totalInstances = total;
              item.activeInstances = active;
            }
  
            // Manually trigger change detection
            this.cdr.markForCheck();
          });
        });
      },
      error: () => this.message.error('Failed to load application types.')
    });
  }  

  deleteTemplate(templateId: string): void {
    this.model.confirm({
      nzTitle: 'Are you sure you want to delete this template?',
      nzOkText: 'Yes',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        this._appTemplateService.deleteAppTemplate(templateId).subscribe(() => {
            this.message.success('Template deleted successfully!');
            this.loadAppTypeList();
          },
          (error: { message: any }) => {
            this.message.remove();
            this.message.error(`Error deleting template: ${error.message || 'Unknown error'}`);
          });
       },
      nzCancelText: 'No',
    });
  }
  
  selectedTemplateId: string | null = null;
  openStageModal(templateId: string) {
    this.selectedTemplateId = templateId;
    this.isVisible = true;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
