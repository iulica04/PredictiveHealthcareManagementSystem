import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { PatientService } from '../../services/patient.service';
import { EmailVerificationService } from '../../services/email-verification.service';
import { PatientCreateComponent } from './patient-create.component';
import { of, throwError } from 'rxjs';

fdescribe('PatientCreateComponent', () => {
  let component: PatientCreateComponent;
  let fixture: ComponentFixture<PatientCreateComponent>;
  let mockPatientService: jasmine.SpyObj<PatientService>;
  let mockEmailVerificationService: jasmine.SpyObj<EmailVerificationService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    mockPatientService = jasmine.createSpyObj('PatientService', ['createPatient']);
    mockEmailVerificationService = jasmine.createSpyObj('EmailVerificationService', ['checkEmailExists']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, PatientCreateComponent],
      providers: [
        { provide: PatientService, useValue: mockPatientService },
        { provide: EmailVerificationService, useValue: mockEmailVerificationService },
        { provide: Router, useValue: mockRouter }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PatientCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize the form with required controls', () => {
    expect(component.patientForm.contains('firstName')).toBeTrue();
    expect(component.patientForm.contains('lastName')).toBeTrue();
    expect(component.patientForm.contains('birthDate')).toBeTrue();
    expect(component.patientForm.contains('gender')).toBeTrue();
    expect(component.patientForm.contains('email')).toBeTrue();
    expect(component.patientForm.contains('phoneNumber')).toBeTrue();
    expect(component.patientForm.contains('address')).toBeTrue();
    expect(component.patientForm.contains('password')).toBeTrue();
    expect(component.patientForm.contains('confirmPassword')).toBeTrue();
  });

  it('should make the firstName control required', () => {
    const control = component.patientForm.get('firstName');
    control?.setValue('');
    expect(control?.valid).toBeFalse();
  });

  it('should validate password match', () => {
    const passwordControl = component.patientForm.get('password');
    const confirmPasswordControl = component.patientForm.get('confirmPassword');
    passwordControl?.setValue('Password1!');
    confirmPasswordControl?.setValue('Password1!');
    component.passwordMatchValidator(component.patientForm);
    expect(confirmPasswordControl?.errors).toBeNull();

    confirmPasswordControl?.setValue('Password2!');
    component.passwordMatchValidator(component.patientForm);
    expect(confirmPasswordControl?.errors).toEqual({ passwordMismatch: true });
  });

  it('should not submit the form if invalid', () => {
    component.patientForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      password: '',
      confirmPassword: ''
    });

    component.onSubmit();

    expect(mockPatientService.createPatient).not.toHaveBeenCalled();
    expect(mockRouter.navigate).not.toHaveBeenCalled();
  });

  it('should submit the form if valid', () => {
    component.patientForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      password: 'Password1!',
      confirmPassword: 'Password1!'
    });

    mockPatientService.createPatient.and.returnValue(of({}));

    component.onSubmit();

    expect(mockPatientService.createPatient).toHaveBeenCalledWith(jasmine.objectContaining({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      password: 'Password1!'
    }));
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should check email existence', () => {
    const emailControl = component.patientForm.get('email');
    emailControl?.setValue('test@example.com');

    mockEmailVerificationService.checkEmailExists.and.returnValue(of(true));

    component.checkEmail();

    expect(mockEmailVerificationService.checkEmailExists).toHaveBeenCalledWith('test@example.com');
    expect(emailControl?.errors).toEqual({ emailExists: true });

    mockEmailVerificationService.checkEmailExists.and.returnValue(of(false));

    component.checkEmail();

    expect(mockEmailVerificationService.checkEmailExists).toHaveBeenCalledWith('test@example.com');
    expect(emailControl?.errors).toBeNull();
  });

  it('should handle email verification error', () => {
    const emailControl = component.patientForm.get('email');
    emailControl?.setValue('test@example.com');

    mockEmailVerificationService.checkEmailExists.and.returnValue(throwError('Error'));

    spyOn(console, 'error');

    component.checkEmail();

    expect(mockEmailVerificationService.checkEmailExists).toHaveBeenCalledWith('test@example.com');
    expect(console.error).toHaveBeenCalledWith('Error checking email', 'Error');
  });

  it('should move to the next step when the current step is valid', () => {
    component.patientForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      password: 'Password1!',
      confirmPassword: 'Password1!'
    });

    component.currentStep = 1;
    component.nextStep();

    expect(component.currentStep).toBe(2);
  });

  it('should not move to the next step if the current step is invalid', () => {
    component.patientForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      password: '',
      confirmPassword: ''
    });

    component.currentStep = 1;
    component.nextStep();

    expect(component.currentStep).toBe(1);
  });

  it('should move to the previous step', () => {
    component.currentStep = 2;
    component.previousStep();

    expect(component.currentStep).toBe(1);
  });

  it('should update the current image based on the step', () => {
    component.currentStep = 1;
    component.updateImage();
    expect(component.currentImage).toBe('assets/images/image2.png');

    component.currentStep = 2;
    component.updateImage();
    expect(component.currentImage).toBe('assets/images/image1.png');

    component.currentStep = 3;
    component.updateImage();
    expect(component.currentImage).toBe('assets/images/image3.png');
  });

  it('should validate password requirements', () => {
    component.patientForm.get('password')?.setValue('Password1!');
    expect(component.hasUpperCase()).toBeTrue();
    expect(component.hasLowerCase()).toBeTrue();
    expect(component.hasNumber()).toBeTrue();
    expect(component.hasSpecialChar()).toBeTrue();
    expect(component.isValidLength()).toBeTrue();
  });

  it('should display password error messages', () => {
    component.patientForm.get('password')?.setValue('pass');
    const errors = component.getPasswordErrors();
    expect(errors).toContain('at least 8 characters');
    expect(errors).toContain('an uppercase letter');
    expect(errors).toContain('a digit');
    expect(errors).toContain('a special character');
  });
});