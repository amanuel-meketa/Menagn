import { Component, inject, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { UserDetailsGetData } from '../../../../models/UserDetailsGetData';

@Component({
  selector: 'app-user-edit',
  standalone: true,
  templateUrl: './user-edit.component.html',
  styleUrls: ['./user-edit.component.css'],
})
export class UserEditComponent implements OnInit {
  private _userService = inject(UserService);
  userDetails: UserDetailsGetData | null = null;

  constructor(private location: Location, private router: Router) {}

  ngOnInit(): void {
    const state = history.state;
    if (state && state['userData']) {
      this.userDetails = state['userData'];
    } else {
      console.error('No user data found in state');
      // Optionally navigate back to previous page or show an error message
    }
  }

  saveChanges(): void {
    if (this.userDetails) {
      // Now userDetails is guaranteed to be non-null
      this._userService.updateUser(this.userDetails.id, this.userDetails).subscribe(
        (response) => {
          console.log('User updated successfully', response);
          // Optionally, navigate back to user details page
          //this.router.navigate(['/user-details', this.userDetails.id]);
        },
        (error) => {
          console.error('Error updating user', error);
          // Handle error (e.g., show an error message)
        }
      );
    } else {
      console.error('User details are null or undefined');
    }
  }
  

  goBack(): void {
    this.location.back();
  }
}
