import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { LoginComponent } from './login.component';
import { LoginService } from '../../services/login.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';

fdescribe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let loginServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    loginServiceMock = jasmine.createSpyObj('LoginService', ['login']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, RouterTestingModule, LoginComponent],
      providers: [
        { provide: LoginService, useValue: loginServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have invalid form when fields are empty', () => {
    expect(component.loginForm.invalid).toBeTrue();
  });

  it('should validate email and password fields', () => {
    const emailField = component.loginForm.get('email');
    const passwordField = component.loginForm.get('password');

    emailField?.setValue('');
    passwordField?.setValue('');
    expect(emailField?.hasError('required')).toBeTrue();
    expect(passwordField?.hasError('required')).toBeTrue();

    emailField?.setValue('invalidEmail');
    passwordField?.setValue('short');
    expect(emailField?.hasError('email')).toBeTrue();
    expect(passwordField?.hasError('minlength')).toBeTrue();
  });

  it('should call login service and navigate to medics on successful admin login', () => {
    loginServiceMock.login.and.returnValue(of({ token: 'token', id: '1', role: 'Admin' }));

    component.loginForm.setValue({ email: 'admin@example.com', password: 'Password1!' });
    component.onSubmit();

    expect(loginServiceMock.login).toHaveBeenCalledWith({ email: 'admin@example.com', password: 'Password1!' });
    expect(routerMock.navigate).toHaveBeenCalledWith(['/medics']);
  });

  it('should call login service and navigate to patients on successful medic login', () => {
    loginServiceMock.login.and.returnValue(of({ token: 'token', id: '2', role: 'Medic' }));

    component.loginForm.setValue({ email: 'medic@example.com', password: 'Password1!' });
    component.onSubmit();

    expect(loginServiceMock.login).toHaveBeenCalledWith({ email: 'medic@example.com', password: 'Password1!' });
    expect(routerMock.navigate).toHaveBeenCalledWith(['/patients']);
  });

  it('should call login service and navigate to patient detail page on successful patient login', () => {
    loginServiceMock.login.and.returnValue(of({ token: 'token', id: '3', role: 'Patient' }));

    component.loginForm.setValue({ email: 'patient@example.com', password: 'Password1!' });
    component.onSubmit();

    expect(loginServiceMock.login).toHaveBeenCalledWith({ email: 'patient@example.com', password: 'Password1!' });
    expect(routerMock.navigate).toHaveBeenCalledWith(['/patients/3']);
  });

  it('should show error message on login failure', () => {
    spyOn(console, 'error');
    loginServiceMock.login.and.returnValue(throwError('Login failed'));

    component.loginForm.setValue({ email: 'invalid@example.com', password: 'invalidPassword' });
    component.onSubmit();

    expect(loginServiceMock.login).toHaveBeenCalledWith({ email: 'invalid@example.com', password: 'invalidPassword' });
    expect(console.error).toHaveBeenCalledWith('Login failed', 'Login failed');
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('should navigate to register page on redirectToRegister call', () => {
    component.redirectToRegister();
    expect(routerMock.navigate).toHaveBeenCalledWith(['patients/register']);
  });

  it('should navigate to forgot password page on redirectToForgotPassword call', () => {
    component.redirectToForgotPassword();
    expect(routerMock.navigate).toHaveBeenCalledWith(['forgot-password']);
  });

  it('should display validation errors for email field', () => {
    const emailField = component.loginForm.get('email');
    emailField?.setValue('');
    emailField?.markAsTouched();
    fixture.detectChanges();
    
    let emailErrors = fixture.debugElement.queryAll(By.css('.error'));
    expect(emailErrors.length).toBeGreaterThan(0);
    expect(emailErrors[0].nativeElement.textContent).toContain('Email is required');

    emailField?.setValue('invalidEmail');
    fixture.detectChanges();
    emailErrors = fixture.debugElement.queryAll(By.css('.error'));
    expect(emailErrors.length).toBeGreaterThan(0);
    expect(emailErrors[0].nativeElement.textContent).toContain('Invalid email format');
  });
});
