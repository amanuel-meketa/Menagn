import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';

import { AppInstanceService } from '../../services/app-instance.service';
import { InstanceList } from '../../../../models/Approval-Instances/InstanceList';
import { NzProgressModule } from 'ng-zorro-antd/progress';

@Component({
  selector: 'app-instance-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NzCardModule,
    NzIconModule,
    NzSelectModule,
    NzAvatarModule,
    NzTagModule,
    NzGridModule,
    NzSpaceModule,
    NzTableModule,
    NzButtonModule,
    NzProgressModule],
  templateUrl: './instance-list.component.html',
  styleUrl: './instance-list.component.css'
})
export class InstanceListComponent implements OnInit {
  private readonly appInstanceService = inject(AppInstanceService);

  // filters
  selectedValue: string | null = null; // templateId
  statusFilter: string | null = null;

  // selection / UI
  selectedInst: InstanceList | null = null;
  approvalComment = '';
  isLoading = false;

  // data
  instances: InstanceList[] = [];
  templates: { label: string; value: string }[] = [];

  // table / sorting helpers exposed to template if you wire them to nzSortFn
  sortByTemplate = (a: InstanceList, b: InstanceList) =>
    this.getTemplateName(a).localeCompare(this.getTemplateName(b));

  sortByCreatedBy = (a: InstanceList, b: InstanceList) =>
    (a.createdBy?.fullName ?? '').localeCompare(b.createdBy?.fullName ?? '');

  sortByStatus = (a: InstanceList, b: InstanceList) =>
    (a.overallStatus ?? '').localeCompare(b.overallStatus ?? '');

  sortByCreatedAt = (a: InstanceList, b: InstanceList) =>
    new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime();

  ngOnInit(): void {
    this.loadInstances();
  }

  loadInstances(): void {
    this.isLoading = true;
    this.appInstanceService.getAllInstances().subscribe({
      next: (res) => {
        this.instances = res ?? [];
        this.loadTemplatesFromInstances();
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  private loadTemplatesFromInstances(): void {
    const uniqueTemplates = new Map<string, string>();
    this.instances.forEach(inst => {
      const tmpl = inst.template;
      if (tmpl && tmpl.templateId && !uniqueTemplates.has(tmpl.templateId)) {
        uniqueTemplates.set(tmpl.templateId, tmpl.name);
      }
    });
    this.templates = Array.from(uniqueTemplates.entries()).map(
      ([value, label]) => ({ label, value })
    );
  }

  clearFilter(): void {
    this.selectedValue = null;
    this.statusFilter = null;
  }

  // used by table template as [nzData]="filteredInstances()"
  filteredInstances(): InstanceList[] {
    const templateId = this.selectedValue;
    const status = this.statusFilter;

    return this.instances.filter(i => {
      const matchesTemplate = templateId ? i.template?.templateId === templateId : true;
      const matchesStatus = status ? i.overallStatus === status : true;
      return matchesTemplate && matchesStatus;
    });
  }

  countByStatus(status: string): number {
    return this.instances.filter(i => i.overallStatus === status).length;
  }

  get filterSummary(): string {
    const parts: string[] = [];
    if (this.selectedValue) {
      const tmpl = this.templates.find(t => t.value === this.selectedValue);
      parts.push(`Template: ${tmpl?.label ?? this.selectedValue}`);
    }
    if (this.statusFilter) {
      parts.push(`Status: ${this.statusFilter}`);
    }
    return parts.length ? `Filtering by ${parts.join(', ')}` : 'Showing all instances';
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Pending': return 'orange';
      case 'Approved': return 'green';
      case 'Rejected': return 'red';
      default: return 'blue';
    }
  }

  getStatusIcon(status: string): string {
    switch (status) {
      case 'Pending': return 'clock-circle';
      case 'Approved': return 'check-circle';
      case 'Rejected': return 'close-circle';
      default: return 'info-circle';
    }
  }

  // safe getter for template name
  getTemplateName(inst: InstanceList): string {
    return inst.template?.name ?? '';
  }

  // called by the table action button
  onViewDetails(inst: InstanceList): void {
    this.selectedInst = inst;
    // set selectedValue to the instance template id so filters / UI stay consistent
    this.selectedValue = inst.template?.templateId ?? null;
    // open modal, navigate, or whatever your app requires
    // e.g. this.router.navigate(['/instances', inst.instanceId]);
  }

  // selection helper (keeps naming explicit)
  selectInstance(instance: InstanceList): void {
    this.selectedInst = instance;
    this.selectedValue = instance.template?.templateId ?? null;
    this.approvalComment = '';
  }

  // trackBy for ngFor / table performance
  trackByInstanceId(_: number, item: InstanceList): string {
    return item.instanceId;
  }

  getProgressPercent(instance: InstanceList): number {
    if (!instance.allStages || instance.allStages === 0) return 0;
    return Math.round((instance.currentStageOrder / instance.allStages) * 100);
  }
}
