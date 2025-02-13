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
import { Router, RouterLink } from '@angular/router';
import { NzBreadCrumbModule } from 'ng-zorro-antd/breadcrumb';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    FormsModule, NzBreadCrumbModule, NzButtonModule, NzDropDownModule, NzIconModule, NzInputModule, RouterLink,
    NzTableModule, NzPageHeaderModule, NzSpaceModule, CommonModule, NzModalModule,
  ],
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css'],
})

export class UserListComponent implements OnInit {
  private _userService = inject(UserService);
  private message = inject(NzMessageService);
  private modal = inject(NzModalService);
  private router = inject(Router);

  searchValue = '';  // Stores the value entered in the search input
  @ViewChild('menu', { static: true }) menu!: NzDropdownMenuComponent;
  isSearchVisible = false;  // Controls visibility of the search dropdown
  originalUserList: UserListData[] = [];  // Stores the original user data
  filteredUserList: UserListData[] = [];  // Stores the filtered user list for display

  ngOnInit(): void {
    this.loadUserList();
  }

  // Fetch the user list from the service
  loadUserList(): void {
    this._userService.getUserList().subscribe(
      (data) => {
        this.originalUserList = data;  // Store the original data
        this.filteredUserList = [...data];  // Copy the data for display
      },
      (error) => {
        console.error('Error fetching user list:', error);
      }
    );
  }

  // Delete a user
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
            this.loadUserList();  // Reload the user list after deletion
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

  // Reset search input
  resetSearch(): void {
    this.searchValue = '';
    this.filterUserList();  // Restore full user list
  }

  // Filter the user list based on search value
  filterUserList(): void {
    this.filteredUserList = this.originalUserList.filter(
      (user: UserListData) => user.username.toLowerCase().includes(this.searchValue.toLowerCase())
    );
  }

  // Toggle search visibility
  toggleSearchVisibility(): void {
    this.isSearchVisible = !this.isSearchVisible;
  }

  navigateToRegister(): void {
    this.router.navigateByUrl('/register').then(() => {
      window.location.reload(); // Forces reload after navigation
    });
  }  
}