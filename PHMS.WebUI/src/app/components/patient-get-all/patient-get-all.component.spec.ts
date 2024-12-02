import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PatientGetAllComponent } from './patient-get-all.component';
import { PatientService } from '../../services/patient.service';
import { of } from 'rxjs';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

fdescribe('PatientGetAllComponent', () => {
  let component: PatientGetAllComponent;
  let fixture: ComponentFixture<PatientGetAllComponent>;
  let patientServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    patientServiceMock = jasmine.createSpyObj('PatientService', ['getAll']);
    patientServiceMock.getAll.and.returnValue(of({
      items: [
        { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St' },
        { id: '2', firstName: 'Jane', lastName: 'Smith', birthDate: '1995-02-10', gender: 'Female', email: 'jane.smith@example.com', phoneNumber: '+0987654321', address: '456 Oak St' }
      ],
      totalCount: 2
    }));

    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [CommonModule, PatientGetAllComponent],
      providers: [
        { provide: PatientService, useValue: patientServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PatientGetAllComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load patients on init', () => {
    expect(patientServiceMock.getAll).toHaveBeenCalledWith(1, 10);
    expect(component.patients.length).toBe(2);
    expect(component.patients[0].firstName).toBe('John');
  });

  it('should load patients on page change', () => {
    component.onPageChange(2);
    expect(patientServiceMock.getAll).toHaveBeenCalledWith(2, 10);
  });
});