import { TestBed, ComponentFixture } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ResetPasswordComponent } from './reset-password.component';
import { PasswordResetService } from '../../services/password-reset.service';
import { RouterTestingModule } from '@angular/router/testing';

fdescribe('ResetPasswordComponent', () => {
  let component: ResetPasswordComponent;
  let fixture: ComponentFixture<ResetPasswordComponent>;
  let passwordResetServiceSpy: jasmine.SpyObj<PasswordResetService>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    passwordResetServiceSpy = jasmine.createSpyObj('PasswordResetService', ['resetPassword']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [
        ResetPasswordComponent, // Import the standalone component
        ReactiveFormsModule,
        RouterTestingModule,
      ],
      providers: [
        { provide: PasswordResetService, useValue: passwordResetServiceSpy },
        { provide: Router, useValue: routerSpy },
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { paramMap: { get: () => 'test-token' } } },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ResetPasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize and get the token from route', () => {
    expect(component.token).toBe('test-token');
  });

  it('should validate password match', () => {
    const passwordControl = component.resetForm.get('newPassword');
    const confirmPasswordControl = component.resetForm.get('confirmPassword');
    passwordControl?.setValue('Password1!');
    confirmPasswordControl?.setValue('Password1!');
    component.passwordMatchValidator(component.resetForm);
    expect(confirmPasswordControl?.errors).toBeNull();

    confirmPasswordControl?.setValue('Password2!');
    component.passwordMatchValidator(component.resetForm);
    expect(confirmPasswordControl?.errors).toEqual({ passwordMismatch: true });
  });

  it('should not reset password if form is invalid', () => {
    component.resetForm.setValue({
      newPassword: '',
      confirmPassword: ''
    });

    component.resetPassword();

    expect(passwordResetServiceSpy.resetPassword).not.toHaveBeenCalled();
    expect(component.errorMessage).toBe('Please fill out the form correctly');
  });

  it('should reset password if form is valid', () => {
    component.resetForm.setValue({
      newPassword: 'Password1!',
      confirmPassword: 'Password1!'
    });

    localStorage.setItem('resetEmail', 'test@example.com');
    passwordResetServiceSpy.resetPassword.and.returnValue(of({ success: true, message: 'Password reset successful' }));

    component.resetPassword();

    expect(passwordResetServiceSpy.resetPassword).toHaveBeenCalledWith('test@example.com', 'test-token', 'Password1!');
    expect(component.resetSuccess).toBeTrue();
    expect(component.errorMessage).toBe('');
  });

  it('should handle reset password error', () => {
    component.resetForm.setValue({
      newPassword: 'Password1!',
      confirmPassword: 'Password1!'
    });

    localStorage.setItem('resetEmail', 'test@example.com');
    passwordResetServiceSpy.resetPassword.and.returnValue(throwError('Error'));

    component.resetPassword();

    expect(passwordResetServiceSpy.resetPassword).toHaveBeenCalledWith('test@example.com', 'test-token', 'Password1!');
    expect(component.errorMessage).toBe('An error occurred while resetting the password');
  });

  it('should navigate to login page', () => {
    component.redirectToLogin();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should validate password requirements', () => {
    component.resetForm.get('newPassword')?.setValue('Password1!');
    expect(component.hasUpperCase()).toBeTrue();
    expect(component.hasLowerCase()).toBeTrue();
    expect(component.hasNumber()).toBeTrue();
    expect(component.hasSpecialChar()).toBeTrue();
    expect(component.isValidLength()).toBeTrue();
  });

  it('should return false for invalid password requirements', () => {
    component.resetForm.get('newPassword')?.setValue('password');
    expect(component.hasUpperCase()).toBeFalse();
    expect(component.hasLowerCase()).toBeTrue();
    expect(component.hasNumber()).toBeFalse();
    expect(component.hasSpecialChar()).toBeFalse();
    expect(component.isValidLength()).toBeTrue();
  });

  it('should return password errors', () => {
    component.resetForm.get('newPassword')?.setValue('password');
    const errors = component.getPasswordErrors();
    expect(errors).toContain('an uppercase letter');
    expect(errors).toContain('a digit');
    expect(errors).toContain('a special character');
  });

  it('should clear error message on successful reset', () => {
    component.resetForm.setValue({
      newPassword: 'Password1!',
      confirmPassword: 'Password1!'
    });

    localStorage.setItem('resetEmail', 'test@example.com');
    passwordResetServiceSpy.resetPassword.and.returnValue(of({ success: true, message: 'Password reset successful' }));

    component.resetPassword();

    expect(component.errorMessage).toBe('');
  });

  it('should set error message on reset failure', () => {
    component.resetForm.setValue({
      newPassword: 'Password1!',
      confirmPassword: 'Password1!'
    });

    localStorage.setItem('resetEmail', 'test@example.com');
    passwordResetServiceSpy.resetPassword.and.returnValue(of({ success: false, message: 'Reset failed' }));

    component.resetPassword();

    expect(component.errorMessage).toBe('Reset failed');
  });

  it('should set error message if email is not in localStorage', () => {
    component.resetForm.setValue({
      newPassword: 'Password1!',
      confirmPassword: 'Password1!'
    });

    localStorage.removeItem('resetEmail');
    component.resetPassword();

    expect(component.errorMessage).toBe('An error occurred while resetting the password');
    expect(passwordResetServiceSpy.resetPassword).not.toHaveBeenCalled();
  });

  it('should set error message if token is not available', () => {
    component.token = '';
    component.resetForm.setValue({
      newPassword: 'Password1!',
      confirmPassword: 'Password1!'
    });

    localStorage.setItem('resetEmail', 'test@example.com');
    component.resetPassword();

    expect(component.errorMessage).toBe('An error occurred while resetting the password');
    expect(passwordResetServiceSpy.resetPassword).not.toHaveBeenCalled();
  });

  it('should set error message if newPassword is not available', () => {
    component.resetForm.setValue({
      newPassword: '',
      confirmPassword: 'Password1!'
    });

    localStorage.setItem('resetEmail', 'test@example.com');
    component.resetPassword();

    expect(component.errorMessage).toBe('Please fill out the form correctly');
    expect(passwordResetServiceSpy.resetPassword).not.toHaveBeenCalled();
  });

  it('should display error message for newPassword field', () => {
    const newPasswordControl = component.resetForm.get('newPassword');
    newPasswordControl?.setValue('');
    newPasswordControl?.markAsTouched();
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.error')?.textContent).toContain('Password is required');
  });

  it('should display error message for confirmPassword field', () => {
    const passwordControl = component.resetForm.get('newPassword');
    const confirmPasswordControl = component.resetForm.get('confirmPassword');
    passwordControl?.setValue('Password1!');
    confirmPasswordControl?.setValue('Password2!');
    confirmPasswordControl?.markAsTouched();
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.error')?.textContent).toContain('Passwords do not match');
  });

  it('should display success message after password reset', () => {
    component.resetSuccess = true;
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.final-message')?.textContent).toContain('Password has been reset successfully');
  });

  it('should disable submit button if form is invalid', () => {
    component.resetForm.setValue({
      newPassword: '',
      confirmPassword: ''
    });
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const button = compiled.querySelector('button[type="submit"]') as HTMLButtonElement;
    expect(button.disabled).toBeTrue();
  });

  it('should enable submit button if form is valid', () => {
    component.resetForm.setValue({
      newPassword: 'Password1!',
      confirmPassword: 'Password1!'
    });
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const button = compiled.querySelector('button[type="submit"]') as HTMLButtonElement;
    expect(button.disabled).toBeFalse();
  });
});