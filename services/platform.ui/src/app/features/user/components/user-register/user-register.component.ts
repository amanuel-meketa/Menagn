import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';

import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzCardModule } from 'ng-zorro-antd/card';
import { AuthService } from '../../../../services/auth.service';
import { RegisterPostData } from '../../models/RegisterPostData';

@Component({
  selector: 'app-user-register',
  standalone: true,
  imports: [ReactiveFormsModule,NzLayoutModule,NzButtonModule,NzFormModule,NzInputModule,NzCardModule,],
  templateUrl: './user-register.component.html',
  styleUrl: './user-register.component.css'
})
export class UserRegisterComponent {
private authService = inject(AuthService); 
  validateForm: FormGroup;

  constructor(private fb: FormBuilder, private router: Router) {
    this.validateForm = this.fb.group({
      firstname: ['', [Validators.required, Validators.maxLength(50)]],
      lastname: ['', [Validators.required, Validators.maxLength(50)]],
      username: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(20)]],
      email: ['', [Validators.required, Validators.email]],
    });
  }

  registerUser(): void {
    if (this.validateForm.valid) {
      const formData: RegisterPostData = this.mapFormValuesToModel();

      // Call AuthService to register the user
      this.authService.registerUser(formData).subscribe({
        next: (response) => {
          console.log('User registered successfully:', response);
          this.router.navigate(['/home']);
        },
        error: (error) => {
          console.error('Registration failed:', error);
        },
      });
    } else {
      console.log('Form is invalid!');
      this.markFormControlsAsDirty();
    }
  }

  private mapFormValuesToModel(): RegisterPostData {
    // Map form values to the RegisterPostData interface
    return {
      firstName: this.validateForm.value.firstname?.trim() || '',
      lastName: this.validateForm.value.lastname?.trim() || '',
      username: this.validateForm.value.username?.trim() || '',
      email: this.validateForm.value.email?.trim() || '',
    };
  }

  private markFormControlsAsDirty(): void {
    // Mark all form controls as dirty and update validity
    Object.values(this.validateForm.controls).forEach((control) => {
      if (control.invalid) {
        control.markAsDirty();
        control.updateValueAndValidity();
      }
    });
  }
}

