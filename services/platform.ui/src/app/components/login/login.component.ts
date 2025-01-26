import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms'; // Import FormBuilder correctly
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, NzLayoutModule, NzButtonModule, NzFormModule, NzInputModule],
  templateUrl: './login.component.html', 
  styleUrls: ['./login.component.css'] 
})
export class LoginComponent {
  validateForm; 
  isCollapsed = false;

  constructor(private fb: FormBuilder, private router: Router) {
    this.validateForm = this.fb.group({
      username: this.fb.control('', [Validators.required]),
      password: this.fb.control('', [Validators.required]),
      remember: this.fb.control(true)
    });
  }

  submitForm(): void {
    if (this.validateForm.valid) {
      console.log('submit', this.validateForm.value);
    } else {
      console.log('Form is invalid!');
    }
  }

  redirectToRegister(): void {
    this.router.navigate(['/register']);
  }
}
