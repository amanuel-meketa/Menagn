import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzTableModule } from 'ng-zorro-antd/table';
import { RoleService } from '../../services/role.service';
import { GetRoleList } from '../../../../models/Role/GetRoleList';

interface CustomColumn {
  name: string;
  value: string;
  required?: boolean;
  position?: 'left' | 'right';
  width?: number;
  fixWidth?: boolean;
  default?: boolean;
}

@Component({
  selector: 'app-role-list',
  standalone: true,
  imports: [ NzButtonModule, NzDividerModule, NzGridModule, NzIconModule, NzModalModule, NzTableModule, CdkDrag, CdkDropList ],
  templateUrl: './role-list.component.html',
  styleUrls: ['./role-list.component.css']
})

export class RoleListComponent implements OnInit {
  listOfData: GetRoleList[] = []; 

  customColumn: CustomColumn[] = [
    {
      name: 'Name',
      value: 'name',
      default: true,
      required: true,
      position: 'left',
      width: 200,
      fixWidth: true
    },
    {
      name: 'Description',
      value: 'description',
      default: true,
      width: 200
    },
    {
      name: 'Action',
      value: 'action',
      default: true,
      required: true,
      position: 'right',
      width: 200
    }
  ];

  isVisible: boolean = false;
  title: CustomColumn[] = [];
  footer: CustomColumn[] = [];
  fix: CustomColumn[] = [];
  notFix: CustomColumn[] = [];

  private _roleList = inject(RoleService);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
      this._roleList.getRoleList().subscribe((roles: GetRoleList[]) => {
      this.listOfData = roles; 
    });
  
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
    this.fix = this.fix.map(item => {
      item.default = true;
      return item;
    });
    this.notFix = this.notFix.map(item => {
      item.default = false;
      return item;
    });
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
}
