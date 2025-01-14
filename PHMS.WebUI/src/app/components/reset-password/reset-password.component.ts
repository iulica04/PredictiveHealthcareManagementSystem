import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PasswordResetService } from '../../services/password-reset.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  resetForm: FormGroup;
  token: string = '';
  resetSuccess: boolean = false;
  errorMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private passwordResetService: PasswordResetService
  ) {
    this.resetForm = this.fb.group({
      newPassword: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.maxLength(100),
          Validators.pattern(/.*[A-Z].*/), // At least one uppercase letter
          Validators.pattern(/.*[a-z].*/), // At least one lowercase letter
          Validators.pattern(/.*[0-9].*/), // At least one digit
          Validators.pattern(/.*[\W_].*/)  // At least one special character
        ]
      ],
      confirmPassword: ['', Validators.required]
    }, { validator: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    this.token = this.route.snapshot.paramMap.get('token') || '';
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('newPassword');
    const confirmPassword = form.get('confirmPassword');
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
    } else {
      confirmPassword?.setErrors(null);
    }
  }

  resetPassword(): void {
    if (this.resetForm.invalid) {
      this.errorMessage = 'Please fill out the form correctly';
      return;
    }

    const email = localStorage.getItem('resetEmail');
    if (!email || !this.token) {
      this.errorMessage = 'An error occurred while resetting the password';
      return;
    }

    const newPassword = this.resetForm.get('newPassword')?.value;
    this.passwordResetService.resetPassword(email, this.token, newPassword).subscribe(
      (response: { success: boolean, message: string }) => {
        if (response.success) {
          this.resetSuccess = true;
          this.errorMessage = '';
        } else {
          this.errorMessage = response.message;
        }
      },
      error => {
        this.errorMessage = 'An error occurred while resetting the password';
      }
    );
  }

  // Functions to check each password validation criteria
  hasUpperCase(): boolean {
    const password = this.resetForm.get('newPassword')?.value;
    return /[A-Z]/.test(password);
  }

  hasLowerCase(): boolean {
    const password = this.resetForm.get('newPassword')?.value;
    return /[a-z]/.test(password);
  }

  hasNumber(): boolean {
    const password = this.resetForm.get('newPassword')?.value;
    return /[0-9]/.test(password);
  }

  hasSpecialChar(): boolean {
    const password = this.resetForm.get('newPassword')?.value;
    return /[\W_]/.test(password);
  }

  isValidLength(): boolean {
    const password = this.resetForm.get('newPassword')?.value;
    return password && password.length >= 8;
  }

  getPasswordErrors(): string[] {
    const errors: string[] = [];
    if (!this.isValidLength()) {
      errors.push('at least 8 characters');
    }
    if (!this.hasUpperCase()) {
      errors.push('an uppercase letter');
    }
    if (!this.hasLowerCase()) {
      errors.push('a lowercase letter');
    }
    if (!this.hasNumber()) {
      errors.push('a digit');
    }
    if (!this.hasSpecialChar()) {
      errors.push('a special character');
    }
    return errors;
  }

  redirectToLogin(): void {
    this.router.navigate(['/login']);
  }
}