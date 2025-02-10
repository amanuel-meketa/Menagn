import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms'; // Import FormBuilder correctly
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { UserService } from '../../services/user.service';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, NzLayoutModule, NzButtonModule, NzFormModule, NzInputModule],
  templateUrl: './login.component.html', 
  styleUrls: ['./login.component.css'] 
})

export class LoginComponent {
  private _userSerivce = inject(UserService);
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
      this._userSerivce.login(this.validateForm.value).subscribe({
        next: (response) => {
          debugger;
          console.log('User logged in successfully:', response);
          this.message.remove();
          this.message.success('logged in successfully!');
          localStorage.setItem('menagn', JSON.stringify(response));
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          console.error('Login failed:', error);
          this.message.remove();
          this.message.error(`Login failed: ${error.message || 'Unknown error'}`);
        },
      });
    } else {
      console.error('Form is invalid');
    }
  }
}
