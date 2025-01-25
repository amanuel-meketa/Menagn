import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms'; // Import FormBuilder correctly
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, NzLayoutModule, NzButtonModule, NzFormModule, NzInputModule],
  templateUrl: './login.component.html', // Your template URL
  styleUrls: ['./login.component.css'] // Your styles URL
})
export class LoginComponent {
  
  validateForm; // Declare validateForm property
  isCollapsed = false;
  constructor(private fb: FormBuilder) {
    // Initialize the form group after fb is injected
    this.validateForm = this.fb.group({
    username: this.fb.control('', [Validators.required]),
    password: this.fb.control('', [Validators.required]),
    remember: this.fb.control(true)
  });
}
  submitForm(): void {
    console.log('submit', this.validateForm.value);
  }
}
