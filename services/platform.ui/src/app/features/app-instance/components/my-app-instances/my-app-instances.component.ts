import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';

import { NzCardModule } from 'ng-zorro-antd/card';
import { NzBreadCrumbModule } from 'ng-zorro-antd/breadcrumb';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzSkeletonModule } from 'ng-zorro-antd/skeleton';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzDividerModule } from 'ng-zorro-antd/divider';

import { AppInstanceService } from '../../services/app-instance.service';
import { InstanceList } from '../../../../models/Approval-Instances/InstanceList';
import { AuthService } from '../../../../shared/services/auth-service.service';

@Component({
  selector: 'app-my-app-instances',
  standalone: true,
  imports: [ CommonModule, FormsModule, NzCardModule, NzBreadCrumbModule, NzSelectModule, NzModalModule,
             NzButtonModule, NzTagModule, NzPaginationModule, NzSkeletonModule, NzToolTipModule, NzDividerModule ],
  templateUrl: './my-app-instances.component.html',
  styleUrl: './my-app-instances.component.css'
})

export class MyAppInstancesComponent implements OnInit {
  private appInstanceService = inject(AppInstanceService);
  private _authService = inject(AuthService);

  // UI state
  searchTerm = '';
  statusFilter: string | null = null;
  sortOrder: 'asc' | 'desc' = 'desc';
  pageIndex = 1;
  pageSize = 6;
  loading = false;

  // data
  instances: InstanceList[] = [];
  modalVisible = false;
  selectedInstance: InstanceList | null = null;

  ngOnInit() {
     const userId = this._authService.getCurrentUserInfoFromToken();
     if (userId?.id) {
        this.loadInstances(userId.id);
    } else {
      console.warn('No user found in token');
    }
  }

  loadInstances(userId: string) {
    this.loading = true;
    this.appInstanceService.getMyInstances(userId).pipe(finalize(() => this.loading = false)).subscribe({
        next: (data) => {
          // ensure dates are Date objects for sorting / checks
          this.instances = (data || []).map(i => ({ ...i }));
          this.sortInstances();
        },
        error: err => {
          console.error('Failed to fetch instances', err);
          this.instances = [];
        }
      });
  }

  private sortInstances() {
    this.instances.sort((a, b) => this.sortOrder === 'asc'
      ? new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
      : new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
    );
    this.pageIndex = 1;
  }

  // view helpers
  getTemplateName(templateId?: string) {
    if (!templateId) return 'Unknown Template';
    const map: Record<string,string> = {
      'leave': 'Leave Request',
      'travel': 'Travel Request',
      'expense': 'Expense Approval'
    };
    return map[templateId] ?? templateId;
  }

  getStatusColor(status?: string): string {
    if (!status) return 'default';
    switch (status.toLowerCase()) {
      case 'pending': return '√';
      case 'approved': return 'warning';
      case 'rejected': return 'processing';
      default: return 'default';
    }
  }

  isNew(instance: InstanceList): boolean {
    const created = new Date(instance.createdAt);
    const sevenDaysAgo = new Date(); sevenDaysAgo.setDate(sevenDaysAgo.getDate() - 7);
    return created >= sevenDaysAgo;
  }

  // computed list for UI (search/filter/paging)
  get filteredInstances(): InstanceList[] {
    let data = this.instances.filter(x => {
      const matchesSearch = this.searchTerm
        ? (x.templateId && x.templateId.toLowerCase().includes(this.searchTerm.toLowerCase()))
          || (x.instanceId && x.instanceId.toLowerCase().includes(this.searchTerm.toLowerCase()))
        : true;
      const matchesStatus = this.statusFilter ? (x.overallStatus === this.statusFilter) : true;
      return matchesSearch && matchesStatus;
    });

    data.sort((a, b) => this.sortOrder === 'asc'
      ? new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
      : new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
    );

    const start = (this.pageIndex - 1) * this.pageSize;
    return data.slice(start, start + this.pageSize);
  }

  // interactions
  openInstance(instance: InstanceList) {
    this.selectedInstance = instance;
    this.modalVisible = true;
  }

  handleCancel() {
    this.modalVisible = false;
    this.selectedInstance = null;
  }

  onPageChange(index: number) {
    this.pageIndex = index;
  }
}
