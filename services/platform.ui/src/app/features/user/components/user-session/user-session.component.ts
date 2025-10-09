import { Component, inject, Input, OnInit } from '@angular/core';
import { UserSession } from '../../../../models/User/UserSession';
import { NzTableModule } from 'ng-zorro-antd/table';
import { UserService } from '../../services/user.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { CommonModule } from '@angular/common';
import { NzModalModule, NzModalService } from 'ng-zorro-antd/modal';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzDividerModule } from 'ng-zorro-antd/divider';

@Component({
    selector: 'app-user-session',
    imports: [NzTableModule, CommonModule, NzModalModule, NzButtonModule, NzDividerModule],
    templateUrl: './user-session.component.html',
    styleUrls: ['./user-session.component.css']
})
export class UserSessionComponent implements OnInit {
  private _userService = inject(UserService);
  private message = inject(NzMessageService);
  private modal = inject(NzModalService);
  @Input() userId!: string;
  listOfData: UserSession[] = [];

  ngOnInit(): void {
    this.getSession();
  }

  getSession(): void {
    if (!this.userId) {
      return;
    }

    this._userService.getUserSeesion(this.userId).subscribe({
      next: (data: UserSession[]) => {
        this.listOfData = data; 
      },
      error: (err) => {
        this.message.error(`Session fetch failed: ${err?.message || 'Unknown error'}`);
      }
    });
  }

  deleteSession(sessionId: string): void {
    this.modal.confirm({
      nzTitle: '<i>Do you Want to delete the session?</i>',
      nzOkText: 'Yes',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () => {
        this._userService.deleteSession(sessionId).subscribe(
          () => {
            this.message.success('Session deleted successfully!');
            this.getSession();  
          },
          (error) => {
            this.message.remove();
            this.message.error(`Error deleting session: ${error.message || 'Unknown error'}`);
          }
        );
      },
      nzCancelText: 'No',
      nzOnCancel: () => console.log('Cancel'),
    });
  }
  
  showDeleteConfirm(): void {
    this.modal.confirm({
      nzTitle: '<i>Do you Want to delete all session?</i>',
      nzContent: '<b>this acction will be kill all the session </b>',
      nzOkText: 'Yes',
      nzOkType: 'primary',
      nzOkDanger: true,
     nzOnOk: () => {
        this._userService.deleteSessions(this.userId).subscribe(
          () => {
            this.message.success('all session deleted successfully!');
          },
          (error) => {
            this.message.remove();
            this.message.error(`Error deleting session: ${error.message || 'Unknown error'}`);
          }
        );
      },
      nzCancelText: 'No',
      nzOnCancel: () => console.log('Cancel'),
    });
  }
}
