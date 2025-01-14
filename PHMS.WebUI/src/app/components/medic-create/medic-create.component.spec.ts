import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { MedicCreateComponent } from './medic-create.component';
import { MedicService } from '../../services/medic.service';
import { By } from '@angular/platform-browser';

fdescribe('MedicCreateComponent', () => {
  let component: MedicCreateComponent;
  let fixture: ComponentFixture<MedicCreateComponent>;
  let medicServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    medicServiceMock = jasmine.createSpyObj('MedicService', ['checkEmailExists', 'createMedic']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, MedicCreateComponent],
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

  it('should validate step 1', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '',
      gender: '',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '',
      password: '',
      confirmPassword: '',
      rank: '',
      specialization: '',
      hospital: ''
    });
    expect(component.validateStep1()).toBeTrue();
  });

  it('should validate step 2', () => {
    component.medicForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '1990-01-01',
      gender: 'Male',
      email: '',
      phoneNumber: '',
      address: '123 Main St',
      password: '',
      confirmPassword: '',
      rank: '',
      specialization: '',
      hospital: ''
    });
    expect(component.validateStep2()).toBeTrue();
  });

  it('should validate step 3', () => {
    component.medicForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      password: 'Password123!',
      confirmPassword: 'Password123!',
      rank: '',
      specialization: '',
      hospital: ''
    });
    expect(component.validateStep3()).toBeTrue();
  });

  it('should validate step 4', () => {
    component.medicForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      password: '',
      confirmPassword: '',
      rank: 'Doctor',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
    });
    expect(component.validateStep4()).toBeTrue();
  });

  it('should navigate to the next step if the current step is valid', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '',
      gender: '',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '',
      password: '',
      confirmPassword: '',
      rank: '',
      specialization: '',
      hospital: ''
    });
    component.nextStep();
    expect(component.currentStep).toBe(2);
  });

  it('should not navigate to the next step if the current step is invalid', () => {
    component.medicForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      password: '',
      confirmPassword: '',
      rank: '',
      specialization: '',
      hospital: ''
    });
    component.nextStep();
    expect(component.currentStep).toBe(1);
  });

  it('should navigate to the previous step', () => {
    component.currentStep = 2;
    component.previousStep();
    expect(component.currentStep).toBe(1);
  });

  it('should check if email exists', () => {
    medicServiceMock.checkEmailExists.and.returnValue(of(false));
    component.medicForm.get('email')?.setValue('john.doe@example.com');
    component.checkEmail();
    expect(medicServiceMock.checkEmailExists).toHaveBeenCalledWith('john.doe@example.com');
  });

  it('should show error if email exists', () => {
    medicServiceMock.checkEmailExists.and.returnValue(of(true));
    component.medicForm.get('email')?.setValue('john.doe@example.com');
    component.checkEmail();
    expect(component.medicForm.get('email')?.hasError('emailExists')).toBeTrue();
  });

  it('should submit the form if valid', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1990-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      password: 'Password123!',
      confirmPassword: 'Password123!',
      rank: 'Doctor',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
    });
    medicServiceMock.createMedic.and.returnValue(of({}));
    component.onSubmit();
    expect(medicServiceMock.createMedic).toHaveBeenCalled();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should not submit the form if invalid', () => {
    component.medicForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      password: '',
      confirmPassword: '',
      rank: '',
      specialization: '',
      hospital: ''
    });
    component.onSubmit();
    expect(medicServiceMock.createMedic).not.toHaveBeenCalled();
  });

  it('should show error message on form submission failure', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1990-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      password: 'Password123!',
      confirmPassword: 'Password123!',
      rank: 'Doctor',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
    });
    medicServiceMock.createMedic.and.returnValue(throwError({ error: 'Error adding medic to database' }));
    component.onSubmit();
    expect(console.error).toHaveBeenCalledWith('Error adding medic to database', { error: 'Error adding medic to database' });
  });
});