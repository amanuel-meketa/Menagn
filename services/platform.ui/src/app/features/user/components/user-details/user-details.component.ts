import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { UserService } from '../../services/user.service';
import { UserDetailsGetData } from '../../../../models/UserDetailsGetData';
import { NzCardModule } from 'ng-zorro-antd/card';

@Component({
  selector: 'app-user-details',
  standalone: true,
  imports: [ CommonModule, NzTabsModule,NzCardModule ,],
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.css'],
})
export class UserDetailsComponent implements OnInit {
  private _userSerivce = inject(UserService);
  userId: string = ''; 
  userDetails: UserDetailsGetData | null = null;

  constructor(private route: ActivatedRoute, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.userId = params['id'];
      this.loadUserDetails();  
    });
  }

  loadUserDetails(): void {
    this._userSerivce.UserDetails(this.userId).subscribe(
      (data: UserDetailsGetData) => {
        this.userDetails = data;
        this.cdr.detectChanges();  
      },
      error => {
        console.error('Error fetching user details', error);
      }
    );
  }
}