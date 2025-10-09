import { ChangeDetectorRef, Component, inject, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { Subject, takeUntil } from 'rxjs';

import { NzCardModule } from 'ng-zorro-antd/card';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzMessageService } from 'ng-zorro-antd/message';
import { RouterModule } from '@angular/router';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';

import { AppInstanceService } from '../../services/app-instance.service';
import { InstanceList } from '../../../../models/Approval-Instances/InstanceList';
import { AuthService } from '../../../../shared/services/auth-service.service';
import { AppTemplateService } from '../../../app-template/services/app-template.service';
import { StageProgressComponent } from '../../../stage-definition/components/stage-progress/stage-progress.component';

@Component({
    selector: 'app-my-app-instances',
    imports: [CommonModule, FormsModule, NzInputModule, NzDatePickerModule, NzSelectModule, NzPaginationModule, NzButtonModule,
        NzIconModule, NzModalModule, NzCardModule, RouterModule, NzTagModule, StageProgressComponent],
    templateUrl: './my-app-instances.component.html',
    styleUrl: './my-app-instances.component.css'
})

export class MyAppInstancesComponent implements OnInit, OnDestroy {
  private readonly _appTemplateService = inject(AppTemplateService);
  private readonly _appInstanceService = inject(AppInstanceService);
  private readonly _authService = inject(AuthService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly message = inject(NzMessageService);
  private readonly model = inject(NzModalService);

  private readonly destroy$ = new Subject<void>();

  // raw data loaded from API
  listOfData: InstanceList[] = [];
  isVisible = false;

  // FILTERS
  templateFilter = '';                 // partial/template name search
  statusFilter: string | null = null;  // 'Pending' | 'Approved' | 'Rejected' | null
  dateRange: Date[] | null = null;     // [start, end] inclusive

  // PAGINATION
  pageIndex = 1;
  pageSize = 6;
  pageSizeOptions = [6, 12, 24];

  // temporary default user id used previously (replace with real auth)
  private readonly defaultUserId = '52b89ab2-b54b-4464-b517-38f82fa10dbd';

  ngOnInit(): void {
    const userId = this._authService.getCurrentUserInfoFromToken()?.id ?? this.defaultUserId;
    this.loadInstances(userId);

    this._appTemplateService.AppTypeListUpdated$.pipe(takeUntil(this.destroy$)).subscribe(() => {
        const user = this._authService.getCurrentUserInfoFromToken();
        this.loadInstances(user?.id ?? this.defaultUserId);
      });
  }

  private loadInstances(userId?: string): void {
    this._appInstanceService.getMyInstances(userId ?? '').pipe(finalize(() => this.cdr.markForCheck())).subscribe({
      next: (data: InstanceList[]) => {
        this.listOfData = data || [];
        this.pageIndex = 1;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Failed to load instances', err);
        this.message.error('Failed to load application instances.');
        this.listOfData = [];
        this.cdr.markForCheck();
      }
    });
  }

  // --- Filtering logic (applied live by template)
  get filteredList(): InstanceList[] {
    let data = (this.listOfData || []).slice();

    // 1) template name filter (case-insensitive substring) — match templateName or templateId
    const name = (this.templateFilter || '').trim().toLowerCase();
    if (name) {
      data = data.filter(d =>
        (d.templateName && d.templateName.toLowerCase().includes(name)) ||
        (d.templateId && d.templateId.toLowerCase().includes(name))
      );
    }

    // 2) status filter (exact match)
    if (this.statusFilter) {
      data = data.filter(d => (d.overallStatus ?? '').toLowerCase() === this.statusFilter!.toLowerCase());
    }

    // 3) date range filter (inclusive)
    if (this.dateRange && this.dateRange.length === 2) {
      const [start, end] = this.dateRange;
      const startTs = new Date(start); startTs.setHours(0, 0, 0, 0);
      const endTs = new Date(end); endTs.setHours(23, 59, 59, 999);

      data = data.filter(d => {
        if (!d.createdAt) return false;
        const created = new Date(d.createdAt).getTime();
        return created >= startTs.getTime() && created <= endTs.getTime();
      });
    }

    return data;
  }

  // page-sliced list used by the template
  get pagedList(): InstanceList[] {
    const items = this.filteredList;
    const start = (this.pageIndex - 1) * this.pageSize;
    return items.slice(start, start + this.pageSize);
  }

  // called when filters change via (ngModelChange) to reset page
  onFilterChange(): void {
    this.pageIndex = 1;
  }

  // pagination handlers
  onPageIndexChange(index: number): void {
    this.pageIndex = index;
    // scroll to top of list (optional)
    const el = document.querySelector('.instance-list');
    if (el) el.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageIndex = 1;
  }

  // UI helpers + actions
  clearFilters(): void {
    this.templateFilter = '';
    this.statusFilter = null;
    this.dateRange = null;
    this.onFilterChange();
  }

  selectedTemplateId: string | null = null;
  openStageModal(templateId: string) {
    this.selectedTemplateId = templateId;
    this.isVisible = true;
  }

  openInstance(instance: InstanceList) {
    this.selectedTemplateId = instance.templateId ?? null;
    this.isVisible = true;
  }

  getStatusColor(status?: string): string {
    if (!status) return 'default';
    switch ((status || '').toLowerCase()) {
      case 'pending': return 'warning';
      case 'approved': return 'green';
      case 'rejected': return 'red';
      default: return 'default';
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
