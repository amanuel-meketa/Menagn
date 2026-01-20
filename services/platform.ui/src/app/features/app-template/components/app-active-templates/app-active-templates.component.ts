import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { Subject, takeUntil } from 'rxjs';
import { GetAppTypeModel } from '../../../../models/Application-Type/GetAppTypeModel';
import { AppTemplateService } from '../../services/app-template.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-active-templates',
    imports: [ CommonModule, FormsModule, NzButtonModule, NzCardModule, NzEmptyModule ],
    templateUrl: './app-active-templates.component.html',
    styleUrl: './app-active-templates.component.css'
})
export class ActiveTemplatesComponent implements OnInit, OnDestroy {
  private readonly _appTemplateService = inject(AppTemplateService);
  private readonly cdr = inject(ChangeDetectorRef)
  private readonly message = inject(NzMessageService);
  private readonly destroy$ = new Subject<void>();
  private readonly router = inject(Router);
  
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

  /** Navigate to StartAppInstanceComponent */
  startApp(template: GetAppTypeModel): void {
    this.router.navigate(['/start-instance', template.templateId]);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
