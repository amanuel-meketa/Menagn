import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router'; // Router imported here
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzSpaceModule } from 'ng-zorro-antd/space'; // Make sure this is included
import { UserListData } from '../../models/UserListData';
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
  searchValue = '';
  visible = false;
  listOfDisplayData: UserListData[] = [];

  constructor(private userService: UserService, private router: Router) {}

  ngOnInit(): void {
    this.fetchUserList();
  }

  fetchUserList(): void {
    this.userService.userList().subscribe(
      (data) => {
        this.listOfDisplayData = data;
        console.log('User fetched successfully:', data.values);
      },
      (error) => {
        console.error('Error fetching user list:', error);
      }
    );
  }

  // Reset the search value and refresh the user list
  reset(): void {
    this.searchValue = '';
    this.fetchUserList();
  }

  // Filter the user list based on the search value
  search(): void {
    this.visible = false;
    const searchValueLower = this.searchValue.trim().toLowerCase();
    this.listOfDisplayData = this.listOfDisplayData.filter((item) =>
      item.username.toLowerCase().includes(searchValueLower)
    );
  }

  addUser(): void {
    this.router.navigate(['/register']);
  }
}
