import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzSkeletonModule } from 'ng-zorro-antd/skeleton';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { Subject, takeUntil } from 'rxjs';
import { GetAppTypeModel } from '../../../../models/Application-Type/GetAppTypeModel';
import { AppTemplateService } from '../../services/app-template.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-active-templates',
    imports: [ CommonModule, FormsModule, NzButtonModule, NzCardModule, NzEmptyModule, NzSkeletonModule, NzPaginationModule, NzSelectModule, NzAvatarModule, NzToolTipModule, NzIconModule ],
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
  isLoading = true;

  // pagination and sorting
  pageIndex = 1;
  pageSize = 9;
  pageSizeOptions = [6, 9, 12];
  sortOption: 'newest' | 'mostUsed' = 'newest';
  selectedCategory = '';

  ngOnInit(): void {
    this.loadAppTypeList();

    this._appTemplateService.AppTypeListUpdated$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.loadAppTypeList());
  }

  private loadAppTypeList(): void {
    this.isLoading = true;
    this._appTemplateService.getAppTemplateList().subscribe({
      next: (data: GetAppTypeModel[]) => {
        this.listOfData = data.map(t => ({ ...t, showFullDescription: false }));
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.message.error('Failed to load templates')
      }
    });
  }
  
  filteredTemplates(): (GetAppTypeModel & { showFullDescription?: boolean })[] {
    if (!this.searchQuery) return this.listOfData;
    return this.listOfData.filter(t =>
      t.name.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }

  displayedTemplates(): (GetAppTypeModel & { showFullDescription?: boolean })[] {
    const filtered = this.filteredTemplates();
    // simple client-side sort
    let sorted = filtered.slice();
    if (this.sortOption === 'newest') {
      // no created date on model - keep original order
    } else if (this.sortOption === 'mostUsed') {
      sorted.sort((a, b) => (b.totalInstances || 0) - (a.totalInstances || 0));
    }
    const start = (this.pageIndex - 1) * this.pageSize;
    return sorted.slice(start, start + this.pageSize);
  }

  onPageChange(page: number): void {
    this.pageIndex = page;
  }

  /** Navigate to StartAppInstanceComponent */
  startApp(template: GetAppTypeModel): void {
    // pass template name as a query param so the start component can display it
    this.router.navigate(['/start-instance', template.templateId], { queryParams: { name: template.name } });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
