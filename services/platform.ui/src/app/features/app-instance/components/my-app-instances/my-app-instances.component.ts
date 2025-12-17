import { ChangeDetectorRef, Component,inject, OnDestroy, OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize, Subject, takeUntil } from 'rxjs';

import { NzCardModule } from 'ng-zorro-antd/card';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { RouterModule } from '@angular/router';

import { AppInstanceService } from '../../services/app-instance.service';
import { AppTemplateService } from '../../../app-template/services/app-template.service';
import { AuthService } from '../../../../shared/services/auth-service.service';
import { InstanceList } from '../../../../models/Approval-Instances/InstanceList';
import { StageProgressComponent } from '../../../stage-definition/components/stage-progress/stage-progress.component';

@Component({
  selector: 'app-my-app-instances',
  standalone: true,
  imports: [ CommonModule, FormsModule, RouterModule, NzInputModule, NzDatePickerModule, NzSelectModule, NzPaginationModule,
             NzButtonModule, NzIconModule,NzModalModule, NzCardModule, NzTagModule,StageProgressComponent],
  templateUrl: './my-app-instances.component.html',
  styleUrl: './my-app-instances.component.css'
})
export class MyAppInstancesComponent implements OnInit, OnDestroy {
  private readonly appInstanceService = inject(AppInstanceService);
  private readonly appTemplateService = inject(AppTemplateService);
  private readonly authService = inject(AuthService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly message = inject(NzMessageService);
  private readonly modal = inject(NzModalService);

  private readonly destroy$ = new Subject<void>();

  // DATA
  listOfData: InstanceList[] = [];

  // FILTERS
  templateFilter = '';
  statusFilter: string | null = null;
  dateRange: Date[] | null = null;

  // PAGINATION
  pageIndex = 1;
  pageSize = 6;
  pageSizeOptions = [6, 12, 24];

  // MODAL
  isVisible = false;
  selectedTemplateId: string | null = null;

ngOnInit(): void {
  const user = this.authService.currentUser;
  if (!user || !user.userId) {
    this.message.error('User not authenticated');
    return;
  }

  // Pass the current user's ID
  this.loadInstances(user.userId);

  // Re-load on template updates
  this.appTemplateService.AppTypeListUpdated$.pipe(takeUntil(this.destroy$))
    .subscribe(() => {
      const refreshedUser = this.authService.currentUser;
      if (refreshedUser && refreshedUser.userId) {
        this.loadInstances(refreshedUser.userId);
      }
    });
}

  private loadInstances(userId: string): void {
    this.appInstanceService
      .getMyInstances(userId)
      .pipe(finalize(() => this.cdr.markForCheck()))
      .subscribe({
        next: (data) => {
          this.listOfData = data ?? [];
          this.pageIndex = 1;
        },
        error: () => {
          this.message.error('Failed to load application instances.');
          this.listOfData = [];
        }
      });
  }

  /* ---------------- FILTERING ---------------- */

  get filteredList(): InstanceList[] {
    let data = [...this.listOfData];

    const search = this.templateFilter.trim().toLowerCase();
    if (search) {
      data = data.filter(d =>
        d.templateName?.toLowerCase().includes(search) ||
        d.templateId?.toLowerCase().includes(search)
      );
    }

    if (this.statusFilter) {
      data = data.filter(d =>
        d.overallStatus?.toLowerCase() === this.statusFilter!.toLowerCase()
      );
    }

    if (this.dateRange?.length === 2) {
      const [start, end] = this.dateRange;
      const startTs = new Date(start).setHours(0, 0, 0, 0);
      const endTs = new Date(end).setHours(23, 59, 59, 999);

      data = data.filter(d => {
        if (!d.createdAt) return false;
        const created = new Date(d.createdAt).getTime();
        return created >= startTs && created <= endTs;
      });
    }

    return data;
  }

  get pagedList(): InstanceList[] {
    const start = (this.pageIndex - 1) * this.pageSize;
    return this.filteredList.slice(start, start + this.pageSize);
  }

  onFilterChange(): void {
    this.pageIndex = 1;
  }

  onPageIndexChange(index: number): void {
    this.pageIndex = index;
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageIndex = 1;
  }

  clearFilters(): void {
    this.templateFilter = '';
    this.statusFilter = null;
    this.dateRange = null;
    this.onFilterChange();
  }

  /* ---------------- UI HELPERS ---------------- */

  openStageModal(templateId: string): void {
    this.selectedTemplateId = templateId;
    this.isVisible = true;
  }

  openInstance(instance: InstanceList): void {
    this.selectedTemplateId = instance.templateId ?? null;
    this.isVisible = true;
  }

  getStatusColor(status?: string): string {
    switch (status?.toLowerCase()) {
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
