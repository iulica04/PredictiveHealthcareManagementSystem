import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PatientListComponent } from './patient-list.component';
import { PatientService } from '../../services/patient.service';
import { of } from 'rxjs';
import { Router } from '@angular/router';
import { Patient } from '../../models/patient.model';
import { CommonModule } from '@angular/common';

fdescribe('PatientListComponent', () => {
  let component: PatientListComponent;
  let fixture: ComponentFixture<PatientListComponent>;
  let patientServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    // Mock PatientService
    patientServiceMock = jasmine.createSpyObj('PatientService', ['getPatients']);
    patientServiceMock.getPatients.and.returnValue(of([
      {
        id: '1',
        firstName: 'John',
        lastName: 'Doe',
        birthDate: '2000-01-01',
        gender: 'Male',
        email: 'john.doe@example.com',
        phoneNumber: '+1234567890',
        address: '123 Main St'
      },
      {
        id: '2',
        firstName: 'Jane',
        lastName: 'Smith',
        birthDate: '1995-02-10',
        gender: 'Female',
        email: 'jane.smith@example.com',
        phoneNumber: '+0987654321',
        address: '456 Oak St'
      }
    ]));

    // Mock Router
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [CommonModule, PatientListComponent],
      providers: [
        { provide: PatientService, useValue: patientServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PatientListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load patients on init', () => {
    expect(patientServiceMock.getPatients).toHaveBeenCalled();
    expect(component.patients.length).toBe(2);
    expect(component.patients[0].firstName).toBe('John');
    expect(component.patients[1].firstName).toBe('Jane');
  });

  it('should navigate to create patient page on navigateToCreatePatient', () => {
    component.navigateToCreatePatient();
    expect(routerMock.navigate).toHaveBeenCalledWith(['patients/create']);
  });

  it('should navigate to patient detail page on navigateToDetailPatient', () => {
    component.navigateToDetailPatient('1');
    expect(routerMock.navigate).toHaveBeenCalledWith(['patients/1']);
  });

  it('should navigate to update patient page on navigateToUpdatePatient', () => {
    component.navigateToUpdatePatient('1');
    expect(routerMock.navigate).toHaveBeenCalledWith(['patients/update/1']);
  });
});