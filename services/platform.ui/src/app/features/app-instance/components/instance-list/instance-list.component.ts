import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzCardModule } from 'ng-zorro-antd/card';

import { AppInstanceService } from '../../services/app-instance.service';

@Component({
    selector: 'app-instance-list',
    imports: [CommonModule, NzCardModule, FormsModule, NzListModule, NzIconModule, NzSelectModule, NzAvatarModule,
        NzTagModule, NzGridModule],
    templateUrl: './instance-list.component.html',
    styleUrls: ['./instance-list.component.css']
})

export class InstanceListComponent implements OnInit {
  private appInstanceService = inject(AppInstanceService);

  selectedValue: string | null = null;
  statusFilter: string | null = null;
  selectedInst: any = null;
  approvalComment: string = '';
  isLoading = false;

  instances: any[] = [];
  templates: { label: string; value: string }[] = [];
  lastUpdated: Date = new Date();

  ngOnInit(): void {
    this.loadInstances();
  }

  loadInstances(): void {
    this.isLoading = true;
    this.appInstanceService.getAllInstances().subscribe({
      next: (res) => {
        this.instances = res;
        this.loadTemplatesFromInstances();
        this.lastUpdated = new Date();
        this.isLoading = false;
      },
      error: () => (this.isLoading = false)
    });
  }

  loadTemplatesFromInstances(): void {
    const uniqueTemplates = new Map<string, string>();
    this.instances.forEach(inst => {
      if (!uniqueTemplates.has(inst.templateId)) {
        uniqueTemplates.set(inst.templateId, inst.templateId);
      }
    });
    this.templates = Array.from(uniqueTemplates.entries()).map(([label, value]) => ({ label, value }));
  }

  clearFilter(): void {
    this.selectedValue = null;
    this.statusFilter = null;
  }

  filteredInstances(): any[] {
    return this.instances.filter(i => {
      const matchesTemplate = this.selectedValue ? i.templateId === this.selectedValue : true;
      const matchesStatus = this.statusFilter ? i.overallStatus === this.statusFilter : true;
      return matchesTemplate && matchesStatus;
    });
  }

  countByStatus(status: string): number {
    return this.instances.filter(i => i.overallStatus === status).length;
  }

  get filterSummary(): string {
    const parts = [];
    if (this.selectedValue) parts.push(`Template: ${this.selectedValue}`);
    if (this.statusFilter) parts.push(`Status: ${this.statusFilter}`);
    return parts.length > 0 ? 'Filtering by ' + parts.join(', ') : 'Showing all instances';
  }
  
  getStatusColor(status: string): string {
    switch (status) {
      case 'Pending':
        return 'orange';
      case 'Approved':
        return 'green';
      case 'Rejected':
        return 'red';
      default:
        return 'blue';
    }
  }
  
  getStatusIcon(status: string): string {
    switch (status) {
      case 'Pending':
        return 'clock-circle';
      case 'Approved':
        return 'check-circle';
      case 'Rejected':
        return 'close-circle';
      default:
        return 'info-circle';
    }
  }
}
