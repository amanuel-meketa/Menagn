import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { UserService } from '../../services/user.service';
import { UserDetailsGetData } from '../../../../models/UserDetailsGetData';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzBreadCrumbModule } from 'ng-zorro-antd/breadcrumb';
import { NzTableModule } from 'ng-zorro-antd/table';
import { FormsModule } from '@angular/forms';

interface EditableField {
  field: string;
  value: string;
  editable: boolean;
  isRestricted: boolean; // Indicates if the field is restricted from editing
}

@Component({
  selector: 'app-user-details',
  standalone: true,
  imports: [CommonModule,RouterLink,NzBreadCrumbModule,NzTabsModule,NzCardModule,NzFormModule,NzButtonModule,NzTableModule,
            FormsModule],
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.css'],
})
export class UserDetailsComponent implements OnInit {
  private _userService = inject(UserService);
  userId: string = '';
  tab: string = '1';
  userDetails: UserDetailsGetData | null = null;
  listOfData: EditableField[] = [];

  constructor(private route: ActivatedRoute, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.userId = params['id'];
      this.loadUserDetails();
    });
  }

  loadUserDetails(): void {
    this._userService.UserDetails(this.userId).subscribe(
      (data: UserDetailsGetData) => {
        this.userDetails = data;
        this.prepareEditableData();
        this.cdr.detectChanges();
      },
      (error) => {
        console.error('Error fetching user details', error);
      }
    );
  }

  prepareEditableData(): void {
    if (this.userDetails) {
      this.listOfData = [
        { field: 'ID', value: this.userDetails.id, editable: false, isRestricted: true },
        { field: 'Username', value: this.userDetails.username, editable: false, isRestricted: true },
        { field: 'First Name', value: this.userDetails.firstName, editable: false, isRestricted: false },
        { field: 'Last Name', value: this.userDetails.lastName, editable: false, isRestricted: false },
        { field: 'Email', value: this.userDetails.email, editable: false, isRestricted: false },
      ];
    }
  }

  startEdit(item: EditableField): void {
    if (!item.isRestricted) {
      item.editable = true;
    }
  }

  saveEdit(item: EditableField): void {
    item.editable = false;
    // Optionally, send the updated data to the server here
  }
}
