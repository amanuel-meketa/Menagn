import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors, ReactiveFormsModule} from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzCardModule } from 'ng-zorro-antd/card';
import { RegisterPostData } from '../../../../models/RegisterPostData';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { Observable, Observer, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzBreadCrumbModule } from 'ng-zorro-antd/breadcrumb';

@Component({
  selector: 'app-user-register',
  standalone: true,
  imports: [ ReactiveFormsModule,NzBreadCrumbModule,NzButtonModule,NzFormModule,NzInputModule,NzLayoutModule,NzModalModule,NzCardModule,CommonModule,RouterLink],
  templateUrl: './user-register.component.html',
  styleUrls: ['./user-register.component.css']
})
export class UserRegisterComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  validateForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private _userService: UserService,
    private message: NzMessageService,
    private router: Router 
  ) {}

  ngOnInit(): void {
    // Update the form with additional fields required by the backend model.
    this.validateForm = this.fb.group({
      userName: ['', [Validators.required],],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.email, Validators.required]],
    });

    // Update confirmation validity when the password changes.
    this.validateForm.controls['password'].valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.validateForm.controls['confirm'].updateValueAndValidity();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  submitForm(): void {
    if (this.validateForm.valid) {
      // Map the form values to the RegisterPostData interface.
      const registerData: RegisterPostData = {
        username: this.validateForm.value.userName,
        firstName: this.validateForm.value.firstName,
        lastName: this.validateForm.value.lastName,
        email: this.validateForm.value.email
      };

      // Call the registration service.
      this._userService.registerUser(registerData).subscribe({
        next: (response) => {
          this.message.remove();
          this.message.success('Registration successful!');
          this.router.navigate(['/list']);
        },
        error: (error) => {
          console.error('Registration failed:', error);
          this.message.remove();
          this.message.error(`Registration failed: ${error.message || 'Unknown error'}`);
        }
      });
    } else {
      Object.values(this.validateForm.controls).forEach(control => {
        control.markAsDirty();
        control.updateValueAndValidity({ onlySelf: true });
      });
    }
  }

  resetForm(e: MouseEvent): void {
    e.preventDefault();
    this.validateForm.reset();
  }
  
  confirmValidator = (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return { error: true, required: true };
    } else if (control.value !== this.validateForm?.value.password) {
      return { confirm: true, error: true };
    }
    return null;
  };
}
