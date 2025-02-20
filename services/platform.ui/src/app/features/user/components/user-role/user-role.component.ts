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
        // Fetch unassigned roles after getting assigned roles
        this.userService.unAssignedUserRole(this.userId).subscribe({
          next: (unassignedRoles: GetRole[]) => {
            this.list = [
              ...assignedRoles.map(role => ({
                key: role.id,
                title: role.name,
                description: role.description,
                direction: 'left' as TransferDirection // Assigned roles on the left
              })),
              ...unassignedRoles.map(role => ({
                key: role.id,
                title: role.name,
                description: role.description,
                direction: 'right' as TransferDirection // Unassigned roles on the right
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
    console.log('nzChange', event);
  }
}
