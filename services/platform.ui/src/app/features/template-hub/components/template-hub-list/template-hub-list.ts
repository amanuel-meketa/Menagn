import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzButtonModule } from 'ng-zorro-antd/button';

import { TemplateIndexItem } from '../../../../models/Template-Hub/TemplateIndexItem';
import { TemplateHub } from '../../services/template-hub';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { FormsModule } from '@angular/forms';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzSkeletonModule } from 'ng-zorro-antd/skeleton';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { Router } from '@angular/router';
@Component({
  selector: 'app-template-hub-list',
  standalone: true,
  imports: [ CommonModule, FormsModule,NzCardModule, NzGridModule, NzIconModule, NzButtonModule, NzBadgeModule,
             NzModalModule, NzInputModule, NzSelectModule, NzAvatarModule, NzTagModule, NzToolTipModule,
             NzPaginationModule, NzSkeletonModule, NzEmptyModule, NzSpinModule],
  templateUrl: './template-hub-list.html',
  styleUrl: './template-hub-list.css'
})
export class TemplateHubListComponent implements OnInit {
  private readonly hub = inject(TemplateHub);
  private readonly modal = inject(NzModalService);
  private readonly router = inject(Router);

  @ViewChild('previewTpl', { static: true }) previewTpl!: TemplateRef<any>;

  templates: TemplateIndexItem[] = [];
  filtered: TemplateIndexItem[] = [];
  loading = true;

  // filters / pagination
  search = '';
  category = 'All';
  categories: string[] = [];
  pageIndex = 1;
  pageSize = 9;

  // UI state
  previewTemplate?: TemplateIndexItem | null = null;
featuredTemplates: any;

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.hub.listIndex().subscribe({
      next: (list) => {
        this.templates = list ?? [];
        this.categories = ['All', ...Array.from(new Set(this.templates.map(t => t.category || 'General')))];
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.templates = [];
        this.filtered = [];
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    const q = this.search?.trim().toLowerCase() ?? '';
    this.filtered = this.templates.filter(t =>
      (this.category === 'All' || (t.category ?? 'General') === this.category) &&
      (!q || (t.name ?? '').toLowerCase().includes(q) || (t.description ?? '').toLowerCase().includes(q))
    );
    this.pageIndex = 1;
  }

  pageData(): TemplateIndexItem[] {
    const start = (this.pageIndex - 1) * this.pageSize;
    return this.filtered.slice(start, start + this.pageSize);
  }
  
  openPreview(template: TemplateIndexItem): void {
    this.router.navigate(['/template-hub-preview', template.key]);
  }
}
