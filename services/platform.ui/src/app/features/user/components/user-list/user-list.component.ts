import { CdkDrag, CdkDragDrop, CdkDropList, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzTableModule } from 'ng-zorro-antd/table';
import { UserService } from '../../services/user.service';
import { CustomColumn } from '../../../../shared/model/custom-column';
import { NzMessageService } from 'ng-zorro-antd/message';
import { UserListData } from '../../../../models/UserListData';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

@Component({
    selector: 'app-user-list',
    imports: [NzButtonModule, NzDividerModule, CommonModule, NzGridModule, NzIconModule, NzModalModule, NzTableModule, CdkDrag, CdkDropList,
        RouterLink],
    templateUrl: './user-list.component.html',
    styleUrls: ['./user-list.component.css']
})
export class UserListComponent implements OnInit {
  private _userService = inject(UserService);  
  private cdr = inject(ChangeDetectorRef);
  private message = inject(NzMessageService);
  private modal = inject(NzModalService); 
  private router = inject(Router);

  listOfData: UserListData[] = []; 

  customColumn: CustomColumn[] = [
    { name: 'Username', value: 'username', default: true, required: true, position: 'left', width: 50, fixWidth: true },  // Update column names for user data
    { name: 'FirstName', value: 'firstName', default: true, width: 50 },  
    { name: 'LastName', value: 'lastName', default: true, width: 50 },  
    { name: 'Email', value: 'email', default: true, width: 50 },  
    { name: 'Action', value: 'action', default: true, required: true, position: 'right', width: 50 }
  ];

  isVisible: boolean = false;
  title: CustomColumn[] = [];
  footer: CustomColumn[] = [];
  fix: CustomColumn[] = [];
  notFix: CustomColumn[] = [];

  ngOnInit(): void {
    this.loadUserList();

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

  deleteUser(userId: string): void { 
    this.modal.confirm({
      nzTitle: 'Are you sure you want to delete this user?',
      nzOkText: 'Yes',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        this._userService.deleteUser(userId).subscribe(
          () => {
            this.message.success('User deleted successfully!');
            this.loadUserList();
          },
          (error: { message: any }) => {
            this.message.remove();
            this.message.error(`Error deleting user: ${error.message || 'Unknown error'}`);
          }
        );
      },
      nzCancelText: 'No',
    });
  }

  private loadUserList(): void {  
    this._userService.getUserList().subscribe((users: UserListData[]) => {
      this.listOfData = users;
      this.cdr.markForCheck();
    });
  }

  navigateToRegister(): void {
    this.router.navigateByUrl('/register').then(() => {
      window.location.reload();
    });
  }  
}