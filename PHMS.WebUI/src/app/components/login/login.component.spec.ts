import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { LoginComponent } from './login.component';
import { LoginService } from '../../services/login.service';
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
      imports: [ReactiveFormsModule, LoginComponent],
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
    component.loginForm.setValue({ email: '', password: '' });
    expect(component.loginForm.invalid).toBeTrue();
  });

  it('should have invalid form when email is invalid', () => {
    component.loginForm.setValue({ email: 'invalid-email', password: 'password' });
    expect(component.loginForm.invalid).toBeTrue();
  });

  it('should have valid form when fields are valid', () => {
    component.loginForm.setValue({ email: 'valid@example.com', password: 'validpassword' });
    expect(component.loginForm.valid).toBeTrue();
  });

  it('should call login service and navigate to medics on successful admin login', () => {
    const loginResponse = { role: 'Admin', token: 'token', id: 1 };
    loginServiceMock.login.and.returnValue(of(loginResponse));
    component.loginForm.setValue({ email: 'admin@example.com', password: 'password' });
    component.onSubmit();
    expect(loginServiceMock.login).toHaveBeenCalledWith({ email: 'admin@example.com', password: 'password' });
    expect(routerMock.navigate).toHaveBeenCalledWith(['/medics']);
  });

  it('should call login service and navigate to medics on successful medic login', () => {
    const loginResponse = { role: 'Medic', token: 'token', id: 2 };
    loginServiceMock.login.and.returnValue(of(loginResponse));
    component.loginForm.setValue({ email: 'medic@example.com', password: 'password' });
    component.onSubmit();
    expect(loginServiceMock.login).toHaveBeenCalledWith({ email: 'medic@example.com', password: 'password' });
    expect(routerMock.navigate).toHaveBeenCalledWith(['/medics/2']);
  });

  it('should call login service and navigate to home on successful patient login', () => {
    const loginResponse = { role: 'Patient', token: 'token', id: 3 };
    loginServiceMock.login.and.returnValue(of(loginResponse));
    component.loginForm.setValue({ email: 'patient@example.com', password: 'password' });
    component.onSubmit();
    expect(loginServiceMock.login).toHaveBeenCalledWith({ email: 'patient@example.com', password: 'password' });
    expect(routerMock.navigate).toHaveBeenCalledWith(['/']);
  });


  it('should navigate to register page on redirectToRegister call', () => {
    component.redirectToRegister();
    expect(routerMock.navigate).toHaveBeenCalledWith(['register']);
  });

  it('should navigate to forgot password page on redirectToForgotPassword call', () => {
    component.redirectToForgotPassword();
    expect(routerMock.navigate).toHaveBeenCalledWith(['forgot-password']);
  });
});