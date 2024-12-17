import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { MedicCreateComponent } from './medic-create.component';
import { MedicService } from '../../services/medic.service';
import { CommonModule } from '@angular/common';
import { By } from '@angular/platform-browser';

fdescribe('MedicCreateComponent', () => {
  let component: MedicCreateComponent;
  let fixture: ComponentFixture<MedicCreateComponent>;
  let medicServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    medicServiceMock = jasmine.createSpyObj('MedicService', ['createMedic', 'checkEmailExists']);
    medicServiceMock.createMedic.and.returnValue(of({}));
    medicServiceMock.checkEmailExists.and.returnValue(of(false));

    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, CommonModule, MedicCreateComponent],
      providers: [
        { provide: MedicService, useValue: medicServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MedicCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize the form with empty values', () => {
    const form = component.medicForm;
    expect(form).toBeTruthy();
    expect(form.get('firstName')?.value).toBe('');
    expect(form.get('lastName')?.value).toBe('');
    expect(form.get('rank')?.value).toBe('');
    expect(form.valid).toBeFalse();
  });

  it('should mark the form as valid with correct values', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      rank: 'Senior', 
      specialization: 'Cardiology',
      hospital: 'Hospital',
      password: 'Password1!',
      confirmPassword: 'Password1!'
    });
    expect(component.medicForm.valid).toBeTrue();
  });

  it('should navigate to the next step on valid step 1 data', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '',
      gender: '',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '',
      rank: '',
      specialization: '',
      hospital: '',
      password: '',
      confirmPassword: ''
    });

    component.nextStep();
    expect(component.currentStep).toBe(2);
  });

  it('should not navigate to the next step on invalid step 1 data', () => {
    component.medicForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      rank: '',
      specialization: '',
      hospital: '',
      password: '',
      confirmPassword: ''
    });

    component.nextStep();
    expect(component.currentStep).toBe(1);
  });

  it('should navigate to the previous step', () => {
    component.currentStep = 2;
    component.previousStep();
    expect(component.currentStep).toBe(1);
  });

  it('should call MedicService.createMedic and navigate on valid form submission', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'Hospital',
      password: 'Password1!',
      confirmPassword: 'Password1!'
    });

    component.onSubmit();

    expect(medicServiceMock.createMedic).toHaveBeenCalledWith({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'Hospital',
      password: 'Password1!'
    });
    expect(routerMock.navigate).toHaveBeenCalledWith(['/medics']);
  });

  it('should not call MedicService.createMedic if form is invalid', () => {
    component.medicForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      rank: '', 
      specialization: '',
      hospital: '',
      password: '',
      confirmPassword: ''
    });

    component.onSubmit();

    expect(medicServiceMock.createMedic).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('should validate password matching', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'Hospital',
      password: 'Password1!',
      confirmPassword: 'Password1'
    });

    component.onSubmit();
    const confirmPasswordErrors = component.medicForm.get('confirmPassword')?.errors || {};
    expect(confirmPasswordErrors['mismatch']).toBeTrue();
    expect(medicServiceMock.createMedic).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('should display validation errors for firstName field', () => {
    const firstNameField = component.medicForm.get('firstName');
    firstNameField?.setValue('');
    firstNameField?.markAsTouched();
    fixture.detectChanges();
    
    const firstNameError = fixture.debugElement.query(By.css('.error')).nativeElement;
    expect(firstNameError.textContent).toContain('First Name is required');
  });

  it('should display validation errors for email field', () => {
    const emailField = component.medicForm.get('email');
    emailField?.setValue('');
    emailField?.markAsTouched();
    fixture.detectChanges();
    
    const emailError = fixture.debugElement.query(By.css('.error')).nativeElement;
    expect(emailError.textContent).toContain('Email is required');
  });

  it('should check email existence on input', () => {
    const emailField = component.medicForm.get('email');
    emailField?.setValue('test@example.com');
    emailField?.markAsTouched();
    component.checkEmail();
    expect(medicServiceMock.checkEmailExists).toHaveBeenCalledWith('test@example.com');
  });

  it('should handle email non-existence validation', () => {
    medicServiceMock.checkEmailExists.and.returnValue(of(false));
    const emailField = component.medicForm.get('email');
    emailField?.setValue('test@example.com');
    component.checkEmail();
    fixture.detectChanges();
    expect(emailField?.errors).toBeNull();
  });
});
