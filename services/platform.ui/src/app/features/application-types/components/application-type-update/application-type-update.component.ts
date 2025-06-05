import { Component, inject, OnInit } from '@angular/core';
import { ApplicationTypeService } from '../../services/application-type.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Router } from '@angular/router';
import { UpdateAppTypeMode } from '../../../../models/Application-Type/UpdateAppTypeMode';
import { GetAppTypeModel } from '../../../../models/Application-Type/GetAppTypeModel';
import { AppTypesharedService } from '../../services/application-type-shared.service';

@Component({
  selector: 'app-application-type-update',
  standalone: true,
  imports: [],
  templateUrl: './application-type-update.component.html',
  styleUrl: './application-type-update.component.css'
})

export class ApplicationTypeUpdateComponent implements OnInit {
  private _appTypeService = inject(ApplicationTypeService);
  private _appTypeSharedService = inject(AppTypesharedService);

  private message = inject(NzMessageService);
  private router = inject(Router);
  appTypeData: UpdateAppTypeMode | null = null;
  
  ngOnInit(): void {
    this.subscribeToAppTypeeData();
  }

  private subscribeToAppTypeeData(): void {
    this._appTypeSharedService.currentAppType$.subscribe({
      next: (data: GetAppTypeModel | null) => {
        if (data) {
          this.appTypeData = {
            id: data.id,
            name: data.name,
            description: data.description
          };
          this.updateRole();
        } else {
          this.message.error('Received null app type data');
        }
      },
      error: (err: any) => {
        this.message.error('Error subscribing to app type data');
        console.error(err);
      }
    });
  }    
  
  
  private updateRole(): void {
    if (!this.appTypeData) {
      this.message.error('App type data is missing or invalid');
      return;
    }

    const updatedAppType = {
      id: this.appTypeData.id,
      name: this.appTypeData.name || '',
      description: this.appTypeData.description || ''
    };

    this._appTypeService.updateAppType(this.appTypeData.id, updatedAppType).subscribe({
      next: (response) => {
        this.message.success('App type updated successfully');
        this.router.navigate(['/app-type-list']);
      },
      error: (err) => {
        console.error('Update app type error:', err);
        const errorMessage = err?.error?.message || err?.message || 'Failed to update app type';
        this.message.error(errorMessage);
      }
    });
  }    
}
