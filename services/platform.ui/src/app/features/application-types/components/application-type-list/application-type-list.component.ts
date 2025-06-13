import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { ChangeDetectorRef, Component, inject, OnInit, OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzTableModule } from 'ng-zorro-antd/table';
import { RoleRegisterComponent } from '../../../role/components/role-register/role-register.component';
import { ApplicationTypeService } from '../../services/application-type.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { GetAppTypeModel } from '../../../../models/Application-Type/GetAppTypeModel';
import { CustomColumn } from '../../../../shared/model/custom-column';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-application-type-list',
  standalone: true,
  imports: [ NzButtonModule, NzDividerModule, NzGridModule, NzIconModule, NzModalModule, NzTableModule,CdkDrag,
              CdkDropList, RoleRegisterComponent, RouterModule ],
  templateUrl: './application-type-list.component.html',
  styleUrl: './application-type-list.component.css'
})
export class ApplicationTypeListComponent implements OnInit, OnDestroy {
  private readonly _appTemplateService = inject(ApplicationTypeService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly message = inject(NzMessageService);
  private readonly destroy$ = new Subject<void>();
  private readonly model = inject(NzModalService);
  listOfData: GetAppTypeModel[] = [];

  customColumn: CustomColumn[] = [
    { name: 'Name', value: 'name', default: true, required: true, position: 'left', width: 100, fixWidth: true },
    { name: 'Description', value: 'description', default: true, width: 400 },
    { name: 'Action', value: 'action', default: true, required: true, position: 'right', width: 50 }
  ];

  isVisible = false;
  title: CustomColumn[] = [];
  footer: CustomColumn[] = [];
  fix: CustomColumn[] = [];
  notFix: CustomColumn[] = [];

  ngOnInit(): void {
    this.loadAppTypeList();

    this.title = this.customColumn.filter(item => item.position === 'left' && item.required);
    this.footer = this.customColumn.filter(item => item.position === 'right' && item.required);
    this.fix = this.customColumn.filter(item => item.default && !item.required);
    this.notFix = this.customColumn.filter(item => !item.default && !item.required);

    this._appTemplateService.AppTypeListUpdated$.pipe(takeUntil(this.destroy$)).subscribe(
      () => this.loadAppTypeList());
   }

  private loadAppTypeList(): void {
    this._appTemplateService.getAppTypeList().subscribe({
      next: (data: GetAppTypeModel[]) => {
        this.listOfData = data;
        this.cdr.markForCheck();
      },
      error: () => this.message.error('Failed to load application types.')
    });
  }

  deleteTemplate(templateId: string): void {
    this.model.confirm({
      nzTitle: 'Are you sure you want to delete this template?',
      nzOkText: 'Yes',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        this._appTemplateService.deleteTemplate(templateId).subscribe(() => {
            this.message.success('Template deleted successfully!');
            this.loadAppTypeList();
          },
          (error: { message: any }) => {
            this.message.remove();
            this.message.error(`Error deleting template: ${error.message || 'Unknown error'}`);
          });
       },
      nzCancelText: 'No',
    });
  }

  drop(event: CdkDragDrop<CustomColumn[]>): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex);
    }

    this.fix = this.fix.map(item => ({ ...item, default: true }));
    this.notFix = this.notFix.map(item => ({ ...item, default: false }));

    this.cdr.markForCheck();
  }

  deleteCustom(value: CustomColumn, index: number): void {
    value.default = false;
    this.notFix = [...this.notFix, value];
    this.fix.splice(index, 1);
    this.cdr.markForCheck();
  }

  addCustom(value: CustomColumn, index: number): void {
    value.default = true;
    this.fix = [...this.fix, value];
    this.notFix.splice(index, 1);
    this.cdr.markForCheck();
  }

  showModal(): void {
    this.isVisible = true;
  }

  handleOk(): void {
    this.customColumn = [...this.title, ...this.fix, ...this.notFix, ...this.footer];
    this.isVisible = false;
    this.cdr.markForCheck();
  }

  handleCancel(): void {
    this.isVisible = false;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
