import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { PatientUpdateComponent } from './patient-update.component';
import { PatientService } from '../../services/patient.service';
import { CommonModule } from '@angular/common';

fdescribe('PatientUpdateComponent', () => {
  let component: PatientUpdateComponent;
  let fixture: ComponentFixture<PatientUpdateComponent>;
  let patientServiceMock: any;
  let routerMock: any;
  let activatedRouteMock: any;

  beforeEach(async () => {
    patientServiceMock = jasmine.createSpyObj('PatientService', ['getById', 'update']);
    patientServiceMock.getById.and.returnValue(of({
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St'
    }));
    patientServiceMock.update.and.returnValue(of({}));

    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy().and.returnValue('1')
        }
      }
    };

    await TestBed.configureTestingModule({
      imports: [CommonModule, ReactiveFormsModule, PatientUpdateComponent],
      providers: [
        { provide: PatientService, useValue: patientServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PatientUpdateComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load patient data on init', () => {
    fixture.detectChanges();
    const token = 'mockToken'; // Tokenul folosit
    expect(patientServiceMock.getById).toHaveBeenCalledWith('1', token);
    expect(component.patientForm.get('firstName')?.value).toBe('John');
    expect(component.patientForm.get('lastName')?.value).toBe('Doe');
  });

  it('should navigate to /patients on error loading patient data', () => {
    patientServiceMock.getById.and.returnValue(throwError('Error loading patient data'));
    component.loadPatientData();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/patients']);
  });

  it('should call update and navigate to /patients on valid form submission', () => {
    fixture.detectChanges();
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

    expect(patientServiceMock.update).toHaveBeenCalledWith('1', {
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      password: 'Password1!'
    }, 'mockToken'); // Adaugă tokenul în așteptările testului
    expect(routerMock.navigate).toHaveBeenCalledWith(['/patients']);
  });

  it('should not load patient data if patientId is missing on route', () => {
    activatedRouteMock.snapshot.paramMap.get.and.returnValue(null);
    component.ngOnInit();
    expect(patientServiceMock.getById).not.toHaveBeenCalled();
  });

  it('should not call update if form is invalid', () => {
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

    expect(patientServiceMock.update).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('should log an error and not navigate on error updating patient', () => {
    fixture.detectChanges();
    const consoleSpy = spyOn(console, 'error');
    patientServiceMock.update.and.returnValue(throwError('Error updating patient'));

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

    expect(patientServiceMock.update).toHaveBeenCalledWith('1', {
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      password: 'Password1!'
    }, 'mockToken'); // Adaugă tokenul în așteptările testului
    expect(consoleSpy).toHaveBeenCalledWith('Error updating patient:', 'Error updating patient');
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });
});
