import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router'; 
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzSpaceModule } from 'ng-zorro-antd/space'; // Make sure this is included
import { UserListData } from '../../../../models/UserListData';
import { UserService } from '../../services/user.service';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-user-list',
  standalone: true,
  imports:
  [ FormsModule, NzButtonModule, NzDropDownModule, NzIconModule, NzInputModule, NzTableModule, NzPageHeaderModule,
    NzSpaceModule, CommonModule,
  ],
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css'],
})

export class UserListComponent implements OnInit {
  _userService = inject(UserService);
  searchValue = '';
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
  reset(): void {
    this.searchValue = '';
    this.search();
  }

  search(): void {
    this.visible = false;
    this.listOfDisplayData = this.listOfDisplayData.filter((item: UserListData) => item.username.indexOf(this.searchValue) !== -1);
  }
}
