import { Component, inject, Input, OnInit } from '@angular/core';
import { UserSession } from '../../../../models/User/UserSession';
import { NzTableModule } from 'ng-zorro-antd/table';
import { UserService } from '../../services/user.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-session',
  standalone: true,
  imports: [NzTableModule, CommonModule], 
  templateUrl: './user-session.component.html',
  styleUrls: ['./user-session.component.css']
})
export class UserSessionComponent implements OnInit {
  private _userService = inject(UserService);
  private message = inject(NzMessageService);
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
  deleteSession(session: UserSession): void {
    // Your logic to delete the session (e.g., calling a service to delete the session)
    console.log('Deleting session:', session);
  }
}
