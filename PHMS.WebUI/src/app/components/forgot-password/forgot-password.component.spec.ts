import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ForgotPasswordComponent } from './forgot-password.component';
import { PasswordResetService } from '../../services/password-reset.service';
import { EmailVerificationService } from '../../services/email-verification.service';
import { FormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';

fdescribe('ForgotPasswordComponent', () => {
  let component: ForgotPasswordComponent;
  let fixture: ComponentFixture<ForgotPasswordComponent>;
  let passwordResetServiceMock: any;
  let emailVerificationServiceMock: any;

  beforeEach(async () => {
    passwordResetServiceMock = jasmine.createSpyObj('PasswordResetService', ['sendResetLink']);
    emailVerificationServiceMock = jasmine.createSpyObj('EmailVerificationService', ['checkEmailExists']);

    await TestBed.configureTestingModule({
      imports: [ForgotPasswordComponent, FormsModule],
      providers: [
        { provide: PasswordResetService, useValue: passwordResetServiceMock },
        { provide: EmailVerificationService, useValue: emailVerificationServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ForgotPasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should send reset link if email exists', () => {
    emailVerificationServiceMock.checkEmailExists.and.returnValue(of(true));
    passwordResetServiceMock.sendResetLink.and.returnValue(of({ success: true, message: 'Email sent' }));

    component.email = 'test@example.com';
    component.sendResetLink();

    expect(emailVerificationServiceMock.checkEmailExists).toHaveBeenCalledWith('test@example.com');
    expect(passwordResetServiceMock.sendResetLink).toHaveBeenCalledWith('test@example.com');
    expect(component.linkSent).toBeTrue();
    expect(component.emailExists).toBeTrue();
  });

  it('should not send reset link if email does not exist', () => {
    emailVerificationServiceMock.checkEmailExists.and.returnValue(of(false));

    component.email = 'test@example.com';
    component.sendResetLink();

    expect(emailVerificationServiceMock.checkEmailExists).toHaveBeenCalledWith('test@example.com');
    expect(passwordResetServiceMock.sendResetLink).not.toHaveBeenCalled();
    expect(component.linkSent).toBeFalse();
    expect(component.emailExists).toBeFalse();
  });

  it('should handle error when checking email existence', () => {
    emailVerificationServiceMock.checkEmailExists.and.returnValue(throwError('Error'));

    component.email = 'test@example.com';
    component.sendResetLink();

    expect(emailVerificationServiceMock.checkEmailExists).toHaveBeenCalledWith('test@example.com');
    expect(passwordResetServiceMock.sendResetLink).not.toHaveBeenCalled();
    expect(component.linkSent).toBeFalse();
    expect(component.emailExists).toBeFalse();
  });

  it('should handle error when sending reset link', () => {
    emailVerificationServiceMock.checkEmailExists.and.returnValue(of(true));
    passwordResetServiceMock.sendResetLink.and.returnValue(throwError('Error'));

    component.email = 'test@example.com';
    component.sendResetLink();

    expect(emailVerificationServiceMock.checkEmailExists).toHaveBeenCalledWith('test@example.com');
    expect(passwordResetServiceMock.sendResetLink).toHaveBeenCalledWith('test@example.com');
    expect(component.linkSent).toBeFalse();
  });

  it('should handle unsuccessful response when sending reset link', () => {
    emailVerificationServiceMock.checkEmailExists.and.returnValue(of(true));
    passwordResetServiceMock.sendResetLink.and.returnValue(of({ success: false, message: 'Email not sent' }));

    component.email = 'test@example.com';
    component.sendResetLink();

    expect(emailVerificationServiceMock.checkEmailExists).toHaveBeenCalledWith('test@example.com');
    expect(passwordResetServiceMock.sendResetLink).toHaveBeenCalledWith('test@example.com');
    expect(component.linkSent).toBeFalse();
    expect(component.emailExists).toBeTrue();
  });

  it('should reset emailExists to true on email input', () => {
    component.emailExists = false;
    const inputElement = fixture.debugElement.query(By.css('input[type="email"]')).nativeElement;

    inputElement.dispatchEvent(new Event('input'));

    expect(component.emailExists).toBeTrue();
  });

  it('should display error message if email does not exist', () => {
    component.emailExists = false;
    fixture.detectChanges();

    const errorMessage = fixture.debugElement.query(By.css('.error')).nativeElement;
    expect(errorMessage).toBeTruthy();
    expect(errorMessage.textContent).toContain("Email doesn't exist");
  });

  it('should display confirmation message if link sent', () => {
    component.linkSent = true;
    fixture.detectChanges();

    const confirmationMessage = fixture.debugElement.query(By.css('.title-container h1')).nativeElement;
    expect(confirmationMessage).toBeTruthy();
    expect(confirmationMessage.textContent).toContain('Check your email');
  });
});
