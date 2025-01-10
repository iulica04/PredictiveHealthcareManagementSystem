import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PatientDetailComponent } from './patient-detail.component';
import { ActivatedRoute, Router } from '@angular/router';
import { PatientService } from '../../services/patient.service';
import { of } from 'rxjs';
import { Patient } from '../../models/patient.model';
import { CommonModule } from '@angular/common';

fdescribe('PatientDetailComponent', () => {
  let component: PatientDetailComponent;
  let fixture: ComponentFixture<PatientDetailComponent>;
  let patientServiceMock: any;
  let routerMock: any;
  let activatedRouteMock: any;
  const mockToken = 'mockToken';

  beforeEach(async () => {
    // Set mock token in sessionStorage
    spyOn(sessionStorage, 'getItem').and.callFake((key) => {
      return key === 'jwtToken' ? mockToken : null;
    });

    patientServiceMock = jasmine.createSpyObj('PatientService', ['getById', 'delete', 'logout']);
    patientServiceMock.getById.and.returnValue(of({
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      patientRecords: [],
      passwordHash: 'hashedPassword'
    }));

    patientServiceMock.delete.and.returnValue(of({}));

    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy().and.returnValue('1')
        }
      }
    };

    await TestBed.configureTestingModule({
      imports: [CommonModule, PatientDetailComponent],
      providers: [
        { provide: PatientService, useValue: patientServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PatientDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch patient details on init', () => {
    expect(patientServiceMock.getById).toHaveBeenCalledWith('1', mockToken);
    expect(component.patient).toEqual({
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      patientRecords: [],
      passwordHash: 'hashedPassword'
    });
  });

  it('should call delete and navigate to patients list on successful deletion', () => {
    component.patient = {
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      patientRecords: [],
      passwordHash: 'hashedPassword'
    };

    component.deletePatient();

    expect(patientServiceMock.delete).toHaveBeenCalledWith('1');
    expect(routerMock.navigate).toHaveBeenCalledWith(['/patients']);
  });

  it('should not call delete if patient is not set', () => {
    component.patient = undefined;

    component.deletePatient();

    expect(patientServiceMock.delete).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });
});