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

 
});