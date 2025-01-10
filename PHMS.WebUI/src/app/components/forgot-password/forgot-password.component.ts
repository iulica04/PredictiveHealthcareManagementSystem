import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent {
  email: string = '';
  linkSent: boolean = false;
  emailExists: boolean = true;

  constructor(
    private authService: AuthService,
  ) {}

  sendResetLink(): void {
    this.authService.checkEmailExists(this.email).subscribe(
      (exists: boolean) => {
        console.log('Email exists:', exists);
        if (exists) {
          this.authService.sendResetLink(this.email).subscribe(
            (resetResponse: { success: boolean, message: string }) => {
              console.log('Password reset response:', resetResponse.message);
              if (resetResponse.success) {
                this.linkSent = true;
                this.emailExists = true;
                localStorage.setItem('resetEmail', this.email);
              } else {
                console.error('Error sending reset link:', resetResponse.message);
              }
            },
            error => {
              console.error('Error sending reset link', error);
            }
          );
        } else {
          this.emailExists = false;
        }
      },
      error => {
        console.error('Error checking email existence', error);
        this.emailExists = false;
      }
    );
  }

  resetEmailExists(): void {
    this.emailExists = true;
  }
}