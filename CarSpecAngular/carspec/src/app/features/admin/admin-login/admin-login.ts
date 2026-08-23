import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginDto } from '../../../core/models/models/loginDto';
import { AuthService } from '../../../core/services/AdminServices/AuthService/authService';

@Component({
  selector: 'app-admin-login',
  imports: [ReactiveFormsModule],
  templateUrl: './admin-login.html',
  styleUrl: './admin-login.css',
})
export class AdminLogin {

  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  showModal = true;

  loginError = signal<string>('');

  loginForm = this.fb.nonNullable.group({
    username: ['', Validators.required],
    password: ['', Validators.required]
  });


  login() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }
    this.loginError.set('');

    const loginDto = new LoginDto();

    loginDto.userName = this.loginForm.value.username;
    loginDto.password = this.loginForm.value.password;

    this.authService.login(loginDto).subscribe({next: response => {
        // Store tokens
        localStorage.setItem('accessToken', response.accessToken
        );
        localStorage.setItem('refreshToken',response.refreshToken
        );
        this.showModal = false;
        this.router.navigate(['/admindashboard']);
        this.authService.isloggedIn.set(true);
      },

      error: error => {
        this.loginError.set(error?.error?.message || error?.error || 'Invalid username or password.');
      }
    })
  }

  closeModal() {
    this.showModal = false;
    this.router.navigateByUrl("/");
  }
}
