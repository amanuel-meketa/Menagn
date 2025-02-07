import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  NonNullableFormBuilder,
  ValidationErrors,
  Validators,
  FormGroup,
  ReactiveFormsModule
} from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzFormModule, NzFormTooltipIcon } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzSelectModule } from 'ng-zorro-antd/select';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [ReactiveFormsModule, NzButtonModule, NzCheckboxModule, NzFormModule, NzInputModule, NzSelectModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent implements OnInit, OnDestroy {
  
  private destroy$ = new Subject<void>();
  validateForm!: FormGroup; // Declare without initializing
  
  constructor(private fb: NonNullableFormBuilder) {}

  ngOnInit(): void {
    // Initialize form in ngOnInit after fb is available
    this.validateForm = this.fb.group({
      email: this.fb.control('', [Validators.email, Validators.required]),
      password: this.fb.control('', [Validators.required]),
      checkPassword: this.fb.control('', [Validators.required, this.confirmationValidator]),
      nickname: this.fb.control('', [Validators.required]),
      phoneNumberPrefix: this.fb.control<'+86' | '+87'>('+86'),
      phoneNumber: this.fb.control('', [Validators.required]),
      website: this.fb.control('', [Validators.required]),
      captcha: this.fb.control('', [Validators.required]),
      agree: this.fb.control(false)
    });

    // Subscribe to password changes
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
      console.log('submit', this.validateForm.value);
    } else {
      Object.values(this.validateForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
    }
  }

  confirmationValidator = (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return { required: true };
    } else if (this.validateForm && control.value !== this.validateForm.controls['password'].value) {
      return { confirm: true, error: true };
    }
    return null;
  };

  getCaptcha(e: MouseEvent): void {
    e.preventDefault();
  }
}
