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
import { NzSpinModule } from 'ng-zorro-antd/spin';

interface EditableField {
  field: string;
  value: string;
  editable: boolean;
  isRestricted: boolean; 
  originalValue: string; 
}

@Component({
  selector: 'app-user-details',
  standalone: true,
  imports: [ CommonModule, RouterLink, NzBreadCrumbModule, NzTabsModule, NzCardModule,NzFormModule, NzButtonModule, NzTableModule,
              FormsModule, NzSpinModule],
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.css'],
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
        { field: 'ID', value: this.userDetails.id, editable: false, isRestricted: true, originalValue: this.userDetails.id },
        { field: 'Username', value: this.userDetails.username, editable: false, isRestricted: true, originalValue: this.userDetails.username },
        { field: 'First Name', value: this.userDetails.firstName, editable: true, isRestricted: false, originalValue: this.userDetails.firstName },
        { field: 'Last Name', value: this.userDetails.lastName, editable: true, isRestricted: false, originalValue: this.userDetails.lastName },
        { field: 'Email', value: this.userDetails.email, editable: true, isRestricted: false, originalValue: this.userDetails.email },
      ];
    }
  }

  startEdit(item: EditableField): void {
    if (!item.isRestricted) {
      item.editable = true;
    }
  }

checkForChanges(): void {
  this.isModified = this.listOfData.some(item => item.value !== item.originalValue);
}


  saveAllChanges(): void {
    const updatedData = {
      firstName: this.listOfData.find(item => item.field === 'First Name')?.value || '',
      lastName: this.listOfData.find(item => item.field === 'Last Name')?.value || '',
      email: this.listOfData.find(item => item.field === 'Email')?.value || ''
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
    // Logic to reset changes, like reverting edited fields to original values
    this.listOfData.forEach(item => {
      item.value = item.originalValue;
      item.editable = false;
    });
    this.isModified = false;
  }  
}
