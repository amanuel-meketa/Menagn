import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { GetStageDefiModel } from '../../../../models/Stage-Definition/GetStageDefiModel';
import { StageDefinitionService } from '../../services/stage-definition.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzModalService, NzModalModule } from 'ng-zorro-antd/modal';
import { AuthService } from '../../../../shared/services/auth-service.service';

@Component({
  selector: 'app-assigned-task',
  standalone: true,
  imports: [ CommonModule, FormsModule, NzCardModule, NzEmptyModule, NzPaginationModule, NzSelectModule, NzTagModule,
             NzButtonModule, NzIconModule, NzModalModule ],
  templateUrl: './assigned-task.component.html',
  styleUrls: ['./assigned-task.component.css']
})

export class AssignedTaskComponent implements OnInit {
  private _stageDefService = inject(StageDefinitionService);
  private _authService = inject(AuthService);
  private modal = inject(NzModalService);

  tasks: GetStageDefiModel[] = [];
  filteredList: GetStageDefiModel[] = [];
  pagedList: GetStageDefiModel[] = [];

  pageIndex = 1;
  pageSize = 5;
  pageSizeOptions = [5, 10, 20];

  searchFilter: string = '';
  assignmentTypeFilter: string | null = null;
  selectedTask: GetStageDefiModel | null = null;
  
 // temporary default user id used previously (replace with real auth)
 private readonly defaultUserId = '52b89ab2-b54b-4464-b517-38f82fa10dbd';

  @ViewChild('taskModal', { static: true }) taskModal!: TemplateRef<any>;

  ngOnInit(): void {
    const userId = this._authService.getCurrentUserInfoFromToken()?.id ?? this.defaultUserId;
    this.loadTasks(userId);
  }

  private loadTasks(userId: string ): void {
    this._stageDefService.assignedTasks(this.defaultUserId).subscribe({
      next: (data: GetStageDefiModel[]) => {
        this.tasks = data;
        this.applyFilters();
      },
      error: (err) => console.error('Failed to load tasks:', err)
    });
  }

  getTagColor(type: string): string {
    switch (type) {
      case 'User': return 'blue';
      case 'Role': return 'green';
      default: return 'default';
    }
  }

  applyFilters(): void {
    this.filteredList = this.tasks.filter(task => {
      const matchesSearch = this.searchFilter
        ? task.stageName?.toLowerCase().includes(this.searchFilter.toLowerCase()) ||
          task.description?.toLowerCase().includes(this.searchFilter.toLowerCase())
        : true;
      const matchesAssignment = this.assignmentTypeFilter
        ? task.assignmentType === this.assignmentTypeFilter
        : true;
      return matchesSearch && matchesAssignment;
    });
    this.pageIndex = 1;
    this.updatePagedList();
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  clearFilters(): void {
    this.searchFilter = '';
    this.assignmentTypeFilter = null;
    this.applyFilters();
  }

  // Open Ng-Zorro modal
  openTask(task: GetStageDefiModel): void {
    this.selectedTask = task;
    this.modal.create({
      nzTitle: task.stageName || 'Task Details',
      nzContent: this.taskModal,
      nzFooter: null,
      nzWidth: 500
    });
  }

  takeAction(task: GetStageDefiModel): void {
    console.log('Take action on task:', task);
    // implement your workflow logic here
  }

  onPageIndexChange(index: number): void {
    this.pageIndex = index;
    this.updatePagedList();
  }

  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageIndex = 1;
    this.updatePagedList();
  }

  private updatePagedList(): void {
    const start = (this.pageIndex - 1) * this.pageSize;
    const end = start + this.pageSize;
    this.pagedList = this.filteredList.slice(start, end);
  }
}
