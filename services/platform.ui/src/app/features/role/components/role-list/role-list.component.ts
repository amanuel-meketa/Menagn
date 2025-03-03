import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzTableModule } from 'ng-zorro-antd/table';
import { RoleService } from '../../services/role.service';
import { GetRoleList } from '../../../../models/Role/GetRoleList';
import { CustomColumn } from '../../../../shared/model/custom-column';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-role-list',
  standalone: true,
  imports: [NzButtonModule, NzDividerModule, NzGridModule, NzIconModule, NzModalModule, NzTableModule, CdkDrag, CdkDropList],
  templateUrl: './role-list.component.html',
  styleUrls: ['./role-list.component.css']
})

export class RoleListComponent implements OnInit {

  private _roleService = inject(RoleService);
  private cdr = inject(ChangeDetectorRef);
  private message = inject(NzMessageService);
  private modal = inject(NzModalService); 

  listOfData: GetRoleList[] = [];
  customColumn: CustomColumn[] = [
    { name: 'Id', value: 'id', default: true, width: 400 },
    { name: 'Name', value: 'name', default: true, required: true, position: 'left', width: 100, fixWidth: true },
    { name: 'Description', value: 'description', default: true, width: 400 },
    { name: 'Action', value: 'action', default: true, required: true, position: 'right', width: 50 }
  ];

  isVisible: boolean = false;
  title: CustomColumn[] = [];
  footer: CustomColumn[] = [];
  fix: CustomColumn[] = [];
  notFix: CustomColumn[] = [];

  ngOnInit(): void {
    this.loadRoleList(); 
    this.loadColumnSettings();
  }

  private categorizeColumns(): void {
    this.title = this.customColumn.filter(item => item.position === 'left' && item.required);
    this.footer = this.customColumn.filter(item => item.position === 'right' && item.required);
    this.fix = this.customColumn.filter(item => item.default && !item.required);
    this.notFix = this.customColumn.filter(item => !item.default && !item.required);
  }

  drop(event: CdkDragDrop<CustomColumn[]>): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex);
    }
    this.updateColumnDefaults();
  }

  private updateColumnDefaults(): void {
    this.fix = this.fix.map(item => {
      item.default = true;
      return item;
    });

    this.notFix = this.notFix.map(item => {
      item.default = false;
      return item;
    });

    this.saveColumnSettings();
    this.cdr.markForCheck();
  }

  deleteCustom(value: CustomColumn, index: number): void {
    value.default = false;
    this.notFix = [...this.notFix, value];
    this.fix.splice(index, 1);
    this.saveColumnSettings();
    this.cdr.markForCheck();
  }

  addCustom(value: CustomColumn, index: number): void {
    value.default = true;
    this.fix = [...this.fix, value];
    this.notFix.splice(index, 1);
    this.saveColumnSettings();
    this.cdr.markForCheck();
  }

  showModal(): void {
    this.isVisible = true;
  }

  handleOk(): void {
    this.customColumn = [...this.title, ...this.fix, ...this.notFix, ...this.footer];
    this.isVisible = false;
    this.saveColumnSettings();
    this.cdr.markForCheck();
  }

  handleCancel(): void {
    this.isVisible = false;
  }

  editRole(data: GetRoleList): void {
    console.log('Edit role:', data);
  }

  deleteRole(userId: string): void {
    this.modal.confirm({
      nzTitle: 'Are you sure you want to delete this role?',
      nzOkText: 'Yes',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        this._roleService.deleteRole(userId).subscribe(
          () => {
            this.message.success('Role deleted successfully!');
            this.loadRoleList();
          },
          (error: { message: any }) => {
            this.message.remove();
            this.message.error(`Error deleting role: ${error.message || 'Unknown error'}`);
          }
        );
      },
      nzCancelText: 'No',
      nzOnCancel: () => console.log('Cancel'),
    });
  }

  private loadRoleList(): void {
    this._roleService.getRoleList().subscribe((roles: GetRoleList[]) => {
      this.listOfData = roles;
      this.cdr.markForCheck();
    });
  }

  private saveColumnSettings(): void {
    const columnSettings = {
      fix: this.fix,
      notFix: this.notFix,
      title: this.title,
      footer: this.footer
    };
    localStorage.setItem('columnSettings', JSON.stringify(columnSettings));  // Save to localStorage
  }

  private loadColumnSettings(): void {
    const savedSettings = localStorage.getItem('columnSettings');
    if (savedSettings) {
      const { fix, notFix, title, footer } = JSON.parse(savedSettings);

      this.fix = fix || [];
      this.notFix = notFix || [];
      this.title = title || [];
      this.footer = footer || [];
      this.customColumn = [...this.title, ...this.fix, ...this.notFix, ...this.footer];

      this.categorizeColumns(); 
    }
  }
}
