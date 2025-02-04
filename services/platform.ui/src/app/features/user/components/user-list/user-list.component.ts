import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDropdownMenuComponent, NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzSpaceModule } from 'ng-zorro-antd/space'; 
import { UserListData } from '../../../../models/UserListData';
import { UserService } from '../../services/user.service';
import { CommonModule } from '@angular/common';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzMessageService } from 'ng-zorro-antd/message';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports:
  [ FormsModule, NzButtonModule, NzDropDownModule, NzIconModule, NzInputModule, NzTableModule, NzPageHeaderModule,
    NzSpaceModule, CommonModule, NzModalModule, RouterLink,
  ],
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css'],
})

export class UserListComponent implements OnInit {
  private _userService = inject(UserService);
  private message = inject(NzMessageService);
  private modal = inject(NzModalService);
  searchValue = ''; 
  @ViewChild('menu', { static: true }) menu!: NzDropdownMenuComponent;
  visible = false;
  listOfDisplayData: UserListData[] = [];

  ngOnInit(): void {
    this.fetchUserList();
  }

  fetchUserList(): void {
    this._userService.userList().subscribe(
      (data) => {
        this.listOfDisplayData = data;
        console.log('User fetched successfully:', data.values);
      },
      (error) => {
        console.error('Error fetching user list:', error);
      }
    );
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
            this.fetchUserList(); 
          },
          (error) => {
            this.message.remove();
            this.message.error(`Error deleting user: ${error.message || 'Unknown error'}`);
          }
        );
      },
      nzCancelText: 'No',
      nzOnCancel: () => console.log('Cancel'),
    });
  }  
  
  reset(): void {
    this.searchValue = '';
    this.search();
  }

  search(): void {
    this.visible = false;
    this.listOfDisplayData = this.listOfDisplayData.filter((item: UserListData) => item.username.indexOf(this.searchValue) !== -1);
  }
}

