import { Component, inject, Input, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzTableModule } from 'ng-zorro-antd/table';
import { AssignedUser } from '../../../../models/Role/AssignedUser';
import { RoleService } from '../../services/role.service';

@Component({
  selector: 'app-role-assigned-users',
  standalone: true,
  imports: [FormsModule, NzButtonModule, NzDropDownModule, NzIconModule, NzInputModule, NzTableModule],
  templateUrl: './role-assigned-users.component.html',
  styleUrls: ['./role-assigned-users.component.css']
})
export class RoleAssignedUsersComponent implements OnInit {
  private _roleService = inject(RoleService);
  
  @Input() roleName!: string;
  searchValue = '';
  visible = false;
  roleAssignedUsers: AssignedUser[] = [];
  listOfDisplayData = [...this.roleAssignedUsers];

  ngOnInit() {
    this.fetchUsersForRole(this.roleName);
  }

  fetchUsersForRole(roleName: string): void {
    this._roleService.fetchUsersForRole(roleName).subscribe({
      next: (users) => {
        this.roleAssignedUsers = users;
        this.listOfDisplayData = [...this.roleAssignedUsers];
      },
      error: (err) => {
        console.error('Error fetching users:', err);
      }
    });
  }

  reset(): void {
    this.searchValue = '';
    this.search();
  }

  search(): void {
    this.visible = false;
    this.listOfDisplayData = this.roleAssignedUsers.filter((item: AssignedUser) => item.username.indexOf(this.searchValue) !== -1);
  }
}
