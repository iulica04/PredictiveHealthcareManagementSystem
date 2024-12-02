import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { PatientCreateComponent } from './patient-create.component';
import { Router } from '@angular/router';
import { PatientService } from '../../services/patient.service';
import { of } from 'rxjs';

fdescribe('PatientCreateComponent', () => {
  let component: PatientCreateComponent;
  let fixture: ComponentFixture<PatientCreateComponent>;
  let patientServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    // Mock PatientService
    patientServiceMock = jasmine.createSpyObj('PatientService', ['createPatient']);
    patientServiceMock.createPatient.and.returnValue(of({}));

    // Mock Router
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule , PatientCreateComponent], // ReactiveFormsModule este necesar pentru formular
      providers: [
        { provide: PatientService, useValue: patientServiceMock },
        { provide: Router, useValue: routerMock }
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PatientCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  fit('should create', () => {
    expect(component).toBeTruthy();
  });

  fit('should initialize the form with empty values', () => {
    const form = component.patientForm;
    expect(form).toBeTruthy();
    expect(form.get('firstName')?.value).toBe('');
    expect(form.get('lastName')?.value).toBe('');
    expect(form.valid).toBeFalse(); // Form should be invalid at the beginning
  });

  fit('should mark the form as valid with correct values', () => {
    component.patientForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      password: 'Password1!'
    });
    expect(component.patientForm.valid).toBeTrue(); // Formularul ar trebui sa fie valid acum
  });

  fit('should call PatientService.createPatient and navigate on valid form submission', () => {
    // Pregătește un formular valid
    component.patientForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      password: 'Password1!'
    });

    component.onSubmit();

    expect(patientServiceMock.createPatient).toHaveBeenCalledWith(component.patientForm.value);
    expect(routerMock.navigate).toHaveBeenCalledWith(['/patients']);
  });

  fit('should not call PatientService.createPatient if form is invalid', () => {
    component.patientForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      password: ''
    });

    component.onSubmit();

    expect(patientServiceMock.createPatient).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });
});