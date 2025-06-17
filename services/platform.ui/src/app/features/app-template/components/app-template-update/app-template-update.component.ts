import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';

import { AppTypesharedService } from '../../services/app-template-shared.service';
import { UpdateAppTypeMode } from '../../../../models/Application-Type/UpdateAppTypeMode';
import { GetAppTypeModel } from '../../../../models/Application-Type/GetAppTypeModel';
import { AppTemplateService } from '../../services/app-template.service';

@Component({
  selector: 'app-app-template-update',
  standalone: true,
  imports: [],
  templateUrl: './app-template-update.component.html',
  styleUrl: './app-template-update.component.css'
})
export class ApplicationTypeUpdateComponent implements OnInit {

  private readonly _appTemplateService = inject(AppTemplateService);
  private readonly appTypeSharedService = inject(AppTypesharedService);
  private readonly messageService = inject(NzMessageService);
  private readonly router = inject(Router);

  appTypeData: UpdateAppTypeMode | null = null;

  ngOnInit(): void {
    this.loadAppTypeData();
  }

  private loadAppTypeData(): void {
    this.appTypeSharedService.currentAppType$.subscribe({
      next: (data: GetAppTypeModel | null) => {
        if (!data) {
          this.messageService.error('Received null app type data');
          return;
        }

        this.appTypeData = {
          templateId: data.templateId,
          name: data.name,
          description: data.description
        };

        this.submitAppTypeUpdate();
      },
      error: (error) => {
        console.error('Failed to retrieve app type data:', error);
        this.messageService.error('Error subscribing to app type data');
      }
    });
  }

  private submitAppTypeUpdate(): void {
    if (!this.appTypeData) {
      this.messageService.error('App type data is missing or invalid');
      return;
    }

    const { templateId, name = '', description = '' } = this.appTypeData;

    const updatedAppType: UpdateAppTypeMode = { templateId, name, description };

    this._appTemplateService.updateAppTemplate(templateId, updatedAppType).subscribe({
      next: () => {
        this.messageService.success('App type updated successfully');
        this.router.navigate(['/app-template-list']);
      },
      error: (error) => {
        console.error('Error updating app type:', error);
        const errorMessage =
          error?.error?.message || error?.message || 'Failed to update app type';
        this.messageService.error(errorMessage);
      }
    });
  }
}
