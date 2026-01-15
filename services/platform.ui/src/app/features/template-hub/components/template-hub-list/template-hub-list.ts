import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { NzCardModule } from 'ng-zorro-antd/card';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzSkeletonModule } from 'ng-zorro-antd/skeleton';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { TemplateHub } from '../../services/template-hub';
import { TemplateIndexItem } from '../../../../models/Template-Hub/TemplateIndexItem';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';

@Component({
  selector: 'app-template-hub-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NzAvatarModule,
    NzCardModule,
    NzGridModule,
    NzButtonModule,
    NzIconModule,
    NzBadgeModule,
    NzTagModule,
    NzSpinModule,
    NzInputModule,
    NzSelectModule,
    NzPaginationModule,
    NzEmptyModule,
    NzSkeletonModule,
    NzToolTipModule
  ],
  templateUrl: './template-hub-list.html',
  styleUrl: './template-hub-list.css'
})
export class TemplateHubListComponent implements OnInit {
  private readonly hub = inject(TemplateHub);
  private readonly router = inject(Router);

  templates: TemplateIndexItem[] = [];
  filtered: TemplateIndexItem[] = [];
  loading = true;

  // filters / pagination
  search = '';
  category = 'All';
  categories: string[] = [];
  pageIndex = 1;
  pageSize = 9;

  // featured templates
  featuredTemplates: TemplateIndexItem[] = [];

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.hub.listIndex().subscribe({
      next: (list) => {
        this.templates = list ?? [];
        this.categories = ['All', ...Array.from(new Set(this.templates.map(t => t.category || 'General')))];
        this.featuredTemplates = this.templates.filter(t => t.featured);
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

  install(template: TemplateIndexItem): void {
    // TODO: implement install logic
    console.log('Install clicked:', template.name);
  }
}
