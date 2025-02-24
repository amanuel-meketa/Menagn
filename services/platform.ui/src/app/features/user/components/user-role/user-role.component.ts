import { Component, inject, Input, OnInit } from '@angular/core';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzTransferModule, TransferItem, TransferDirection } from 'ng-zorro-antd/transfer';
import { UserService } from '../../services/user.service';
import { GetRole } from '../../../../models/User/GetRole';

@Component({
  selector: 'app-user-role',
  standalone: true,
  imports: [NzButtonModule, NzTransferModule],
  templateUrl: './user-role.component.html',
  styleUrls: ['./user-role.component.css']
})
export class UserRoleComponent implements OnInit {
  private messageService = inject(NzMessageService);
  private userService = inject(UserService);

  @Input() userId!: string;
  list: TransferItem[] = [];

  ngOnInit(): void {
    this.getData();
  }

  getData(): void {
    if (!this.userId) {
      return;
    }

    this.userService.assignedUserRole(this.userId).subscribe({
      next: (assignedRoles: GetRole[]) => {
        this.userService.unAssignedUserRole(this.userId).subscribe({
          next: (unassignedRoles: GetRole[]) => {
            // Left side -> Unassigned roles, Right side -> Assigned roles
            this.list = [
              ...unassignedRoles.map(role => ({
                key: role.id, 
                title: role.name,
                description: role.description,
                direction: 'left' as TransferDirection // Available roles on the left
              })),
              ...assignedRoles.map(role => ({
                key: role.id, 
                title: role.name,
                description: role.description,
                direction: 'right' as TransferDirection // Assigned roles on the right
              }))
            ];
          },
          error: (err) => {
            this.messageService.error(`Failed to fetch unassigned roles: ${err.message || 'Unknown error'}`);
          }
        });
      },
      error: (err) => {
        this.messageService.error(`Failed to fetch assigned roles: ${err.message || 'Unknown error'}`);
      }
    });
  }

  reload(): void {
    this.getData();
    this.messageService.success('Roles reloaded successfully!');
  }

  select(event: any): void {
    console.log('nzSelectChange', event);
  }

  change(event: any): void {
    console.log('nzChange event:', event);

    if (!event || !Array.isArray(event.list)) {
      console.error('List is not an array or is undefined');
      return;
    }

    // Map the roles to be assigned or unassigned based on the direction
    const rolesToAssign: GetRole[] = event.list
      .filter((role: TransferItem) => role.direction === 'right') // Move from left to right for assignment
      .map((role: TransferItem) => ({
        id: role['key'],  
        name: role.title,
        description: role['description']
      }));

    const rolesToUnassign: GetRole[] = event.list
      .filter((role: TransferItem) => role.direction === 'left') // Move from right to left for unassignment
      .map((role: TransferItem) => ({
        id: role['key'],  
        name: role.title,
        description: role['description']
      }));

    console.log('Roles to assign:', rolesToAssign);
    console.log('Roles to unassign:', rolesToUnassign);

    // Call API for assigning roles if there are any roles to assign
    if (rolesToAssign.length > 0) {
      this.assignRoles(rolesToAssign);
    }

    // Call API for unassigning roles if there are any roles to unassign
    if (rolesToUnassign.length > 0) {
      this.unassignRoles(rolesToUnassign);
    }
  }

  // Method to assign roles
  assignRoles(roles: GetRole[]): void {
    this.userService.assignUserRoles(this.userId, roles).subscribe({
      next: (response) => {
        console.log('Backend response for assignRoles:', response);
        this.messageService.success('Roles assigned successfully!');
        this.getData(); // Reload the data after assigning roles
      },
      error: (err) => {
        console.error('Error assigning roles:', err);
        this.messageService.error(`Failed to assign roles: ${err.message || 'Unknown error'}`);
      }
    });
  }

  // Method to unassign roles
  unassignRoles(roles: GetRole[]): void {
    this.userService.unassignUserRoles(this.userId, roles).subscribe({
      next: (response) => {
        console.log('Backend response for unassignRoles:', response);
        this.messageService.success('Roles unassigned successfully!');
        this.getData(); // Reload the data after unassigning roles
      },
      error: (err) => {
        console.error('Error unassigning roles:', err);
        this.messageService.error(`Failed to unassign roles: ${err.message || 'Unknown error'}`);
      }
    });
  }
}
