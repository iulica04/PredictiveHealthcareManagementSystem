import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  submitted: boolean = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    this.submitted = true;
    if (this.loginForm.valid) {
      this.authService.login(this.loginForm.value).subscribe({
        next: (response: any) => {
          console.log('Login successful', response);
  
          // Store the JWT token & use role in sessionStorage
          sessionStorage.setItem('jwtToken', response.token);
          sessionStorage.setItem('userId', response.id);
          sessionStorage.setItem('role', response.role);
  
          // Redirect the user after authentication
          // WIP only redirects to user details page
          this.router.navigate([`users/${response.id}`]);
        },
        error: (error: any) => {
          console.error('Login failed', error);
        }
      });
    }
  }

  redirectToRegister(): void {
    this.router.navigate(['patients/register']); // Redirecționează utilizatorul la pagina de înregistrare
  }

  redirectToForgotPassword(): void {
    this.router.navigate(['forgot-password']); // Redirecționează utilizatorul la pagina de resetare a parolei
  }

}