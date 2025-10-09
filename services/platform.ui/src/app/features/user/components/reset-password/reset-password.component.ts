import { Component, inject, OnDestroy, OnInit, Input } from '@angular/core';
import { FormGroup, Validators, AbstractControl, ValidationErrors, ReactiveFormsModule, NonNullableFormBuilder } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { UserService } from '../../services/user.service';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
    selector: 'app-reset-password',
    imports: [ReactiveFormsModule, NzButtonModule, NzFormModule, NzInputModule],
    templateUrl: './reset-password.component.html',
    styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit, OnDestroy {
  private _userService = inject(UserService);
  private message = inject(NzMessageService);
  private fb = inject(NonNullableFormBuilder);
  private destroy$ = new Subject<void>();
  validateForm!: FormGroup;

  @Input() userId!: string; 

  ngOnInit(): void {
      this.validateForm = this.fb.group({
      password: this.fb.control('', [Validators.required]),
      checkPassword: this.fb.control('', [Validators.required, this.confirmationValidator]),
    });

      this.validateForm.controls['password'].valueChanges.pipe(takeUntil(this.destroy$)).subscribe(() => {
      this.validateForm.controls['checkPassword'].updateValueAndValidity();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  submitForm(): void {
    if (this.validateForm.valid) {
      const newPassword = this.validateForm.controls['password'].value;

      if (!this.userId) {
        this.message.error('User ID is missing. Please try again.');
        return;
      }

      this._userService.resetPassword(this.userId, newPassword).subscribe({
        next: () => {
          this.message.success('Password reset successful!');
        },
        error: (err) => {
          this.message.error(`Password reset failed: ${err?.message || 'Unknown error'}`);
        }
      });
    } else {
      this.validateForm.markAllAsTouched();
      this.message.error('Please fill in all required fields correctly.');
    }
  }

  confirmationValidator = (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return { required: true };
    }
    const password = this.validateForm?.controls['password']?.value;
    if (control.value !== password) {
      return { confirm: true, error: true };
    }
    return null;
  };
}
