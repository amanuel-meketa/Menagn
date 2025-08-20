import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { Subject, takeUntil } from 'rxjs';
import { GetAppTypeModel } from '../../../../models/Application-Type/GetAppTypeModel';
import { TemplateStagesComponent } from '../../../stage-definition/components/template-stages/template-stages.component';
import { AppTemplateService } from '../../services/app-template.service';
import { AppTemplateCreateComponent } from '../app-template-create/app-template-create.component';

@Component({
  selector: 'app-active-templates',
  standalone: true,
  imports: [ CommonModule, FormsModule, RouterModule, NzButtonModule, NzCardModule, NzGridModule, NzIconModule,
             NzInputModule, NzModalModule, NzTagModule, AppTemplateCreateComponent, TemplateStagesComponent ],
  templateUrl: './app-active-templates.component.html',
  styleUrls: ['./app-active-templates.component.css']
})
export class ActiveTemplatesComponent implements OnInit, OnDestroy {
  private readonly _appTemplateService = inject(AppTemplateService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly message = inject(NzMessageService);
  private readonly destroy$ = new Subject<void>();
  private readonly modal = inject(NzModalService);

  listOfData: (GetAppTypeModel & { showFullDescription?: boolean })[] = [];
  searchQuery = '';

  ngOnInit(): void {
    this.loadAppTypeList();

    this._appTemplateService.AppTypeListUpdated$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.loadAppTypeList());
  }

  private loadAppTypeList(): void {
    this._appTemplateService.getAppTemplateList().subscribe({
      next: (data: GetAppTypeModel[]) => {
        this.listOfData = data.map(t => ({ ...t, showFullDescription: false }));
        this.cdr.detectChanges();
      },
      error: () => this.message.error('Failed to load templates')
    });
  }

  filteredTemplates(): (GetAppTypeModel & { showFullDescription?: boolean })[] {
    if (!this.searchQuery) return this.listOfData;
    return this.listOfData.filter(t =>
      t.name.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }

  startInstance(template: GetAppTypeModel): void {
    // Logic to start an instance based on the selected template
    this.message.success(`Starting instance for: ${template.name}`);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
