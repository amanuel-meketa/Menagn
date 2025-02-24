import { Component, inject, OnInit, ChangeDetectorRef, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
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
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { ResetPasswordComponent } from '../reset-password/reset-password.component';
import { UserSessionComponent } from '../user-session/user-session.component';
import { UserRoleComponent } from '../user-role/user-role.component';
import { EditableField } from '../../../../shared/model/EditableField';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-details',
  standalone: true,
  imports: [CommonModule, RouterLink, NzBreadCrumbModule, NzTabsModule, NzCardModule, NzFormModule, NzButtonModule, NzTableModule,
            FormsModule, NzSpinModule, ResetPasswordComponent, UserSessionComponent, UserRoleComponent, NzIconModule,
            NzSwitchModule],
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.css'],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]  // Add this line
})

export class UserDetailsComponent implements OnInit {
  private _userService = inject(UserService);
  userId: string = '';
  tab: string = '1';
  userDetails: UserDetailsGetData | null = null;
  listOfData: EditableField[] = [];
  isModified: boolean = false;

  constructor(private route: ActivatedRoute, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.userId = params['id'];
      this.loadUserDetails();
    });
  }

  loadUserDetails(): void {
    this._userService.getUserDetails(this.userId).subscribe(
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
        { field: 'ID', value: this.userDetails.id, editable: false, isRestricted: true, originalValue: this.userDetails.id, inputType: 'text' },
        { field: 'Username', value: this.userDetails.username, editable: false, isRestricted: true, originalValue: this.userDetails.username, inputType: 'text' },
        { field: 'First Name', value: this.userDetails.firstName, editable: true, isRestricted: false, originalValue: this.userDetails.firstName, inputType: 'text' },
        { field: 'Last Name', value: this.userDetails.lastName, editable: true, isRestricted: false, originalValue: this.userDetails.lastName, inputType: 'text' },
        { field: 'Email', value: this.userDetails.email, editable: true, isRestricted: false, originalValue: this.userDetails.email, inputType: 'text' },
        { field: 'Email Verified', value: this.userDetails.emailVerified, editable: true, isRestricted: false, originalValue: this.userDetails.emailVerified, inputType: 'boolean' },
        { field: 'Enabled', value: this.userDetails.enabled, editable: true, isRestricted: false, originalValue: this.userDetails.enabled, inputType: 'boolean' },
        { field: 'Created Timestamp', value: this.userDetails.createdTimestamp, editable: false, isRestricted: true, originalValue: this.userDetails.createdTimestamp, inputType: 'text' },
      ];
    }
  }

  checkForChanges(): void {
    this.isModified = this.listOfData.some(item => item.value !== item.originalValue);
  }

  saveAllChanges(): void {
    const updatedData = {
      firstName: this.listOfData.find(item => item.field === 'First Name')?.value || '',
      lastName: this.listOfData.find(item => item.field === 'Last Name')?.value || '',
      email: this.listOfData.find(item => item.field === 'Email')?.value || '',
      emailVerified: this.listOfData.find(item => item.field === 'Email Verified')?.value as boolean,  // Directly cast to boolean
      enabled: this.listOfData.find(item => item.field === 'Enabled')?.value as boolean,  // Directly cast to boolean
    };
  
    console.log('Sending data:', JSON.stringify(updatedData, null, 2));
  
    this._userService.updateUser(this.userId, updatedData).subscribe(
      (response) => {
        console.log('User details updated successfully!', response);
      },
      (error) => {
        console.error('Error updating user details', error);
        if (error.error && error.error.errors) {
          console.error('Validation Errors:', error.error.errors);
        }
      }
    );
  }

  resetChanges(): void {
    this.listOfData.forEach(item => {
      item.value = item.originalValue;
      item.editable = false;
    });
    this.isModified = false;
  }
}
