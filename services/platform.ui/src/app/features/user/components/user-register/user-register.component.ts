import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzCardModule } from 'ng-zorro-antd/card';
import { RegisterPostData } from '../../../../models/RegisterPostData';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-user-register',
  standalone: true,
  imports: [ReactiveFormsModule, NzLayoutModule, NzButtonModule, NzFormModule, NzInputModule, NzCardModule, CommonModule],
  templateUrl: './user-register.component.html',
  styleUrls: ['./user-register.component.css']
})
export class UserRegisterComponent {
  private _userSerivce = inject(UserService);
  private message = inject(NzMessageService);
  validateForm: FormGroup;

  constructor(private fb: FormBuilder, private router: Router) {
    this.validateForm = this.fb.group({
      firstname: ['', [Validators.required, Validators.maxLength(50)]],
      lastname: ['', [Validators.required, Validators.maxLength(50)]],
      username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(20)]],
      email: ['', [Validators.required, Validators.email]],
    });
  }

  registerUser(): void {
    if (this.validateForm.valid) {
      const formData: RegisterPostData = this.mapFormValuesToModel();

      const loadingMessage = this.message.loading('Registering user...', { nzDuration: 0 });
      
      this._userSerivce.registerUser(formData).subscribe({ next: (response) => {
           this.message.remove();
          this.message.success('Registration successful!');
          this.router.navigate(['/list']);
        },
        error: (error) => {
          console.error('Registration failed:', error);
          this.message.remove();
          this.message.error(`Registration failed: ${error.message || 'Unknown error'}`);
        },
      });
    } else {
      console.log('Form is invalid!');
      this.markFormControlsAsDirty();
    }
  }

  private mapFormValuesToModel(): RegisterPostData {
    return {
      firstName: this.validateForm.value.firstname?.trim() || '',
      lastName: this.validateForm.value.lastname?.trim() || '',
      username: this.validateForm.value.username?.trim() || '',
      email: this.validateForm.value.email?.trim() || '',
    };
  }

  private markFormControlsAsDirty(): void {
    Object.values(this.validateForm.controls).forEach((control) => {
      if (control.invalid) {
        control.markAsDirty();
        control.updateValueAndValidity();
      }
    });
  }
}
