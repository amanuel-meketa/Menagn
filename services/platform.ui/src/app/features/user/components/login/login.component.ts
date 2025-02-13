import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { UserService } from '../../services/user.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { AuthService } from '../../../../shared/services/auth-service.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, NzLayoutModule, NzButtonModule, NzFormModule, NzInputModule],
  templateUrl: './login.component.html', 
  styleUrls: ['./login.component.css'] 
})

export class LoginComponent {
  private readonly _userService = inject(UserService);
  private readonly _authService = inject(AuthService);
  private message = inject(NzMessageService);
  private router = inject(Router);
  validateForm: FormGroup;
  isCollapsed = false;

  constructor( private fb: FormBuilder,){
      this.validateForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(20)]],
      password: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
    });
  }

  login(): void {
    if (this.validateForm.valid) {
      const loadingMessage = this.message.loading('User logging...', { nzDuration: 0 });
  
      this._userService.login(this.validateForm.value).subscribe({
        next: (response) => {
          this.message.remove();
          if (response?.access_token) {
            this.message.success('Logged in successfully!');
            localStorage.setItem('Bearer', response.access_token); // ✅ Store only access_token
            this._authService.currentUserSig.set(response.access_token);
            this.router.navigate(['/dashboard']);
          } else {
            this.message.error('Login failed: Access token missing.');
          }
        },
        error: (error) => {
          this.message.remove();
          console.error('Login failed:', error);
          this.message.error(`Login failed: ${error.message || 'Unknown error'}`);
        }
      });
    } else {
      console.error('Form is invalid');
      this.message.error('Please fill in all required fields.');
    }
  }
  
}
