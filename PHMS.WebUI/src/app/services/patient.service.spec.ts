import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PatientService } from './patient.service';
import { Patient } from '../models/patient.model';
import { Consultation, ConsultationStatus } from '../models/consultation.model';
import { Router } from '@angular/router';

fdescribe('PatientService', () => {
  let service: PatientService;
  let httpMock: HttpTestingController;
  let routerMock: any;

  beforeEach(() => {
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PatientService, { provide: Router, useValue: routerMock }]
    });

    service = TestBed.inject(PatientService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get patients', () => {
    const mockPatients: Patient[] = [{
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1980-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      passwordHash: 'hashedPassword',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      patientRecords: []
    }];

    service.getPatients().subscribe((patients) => {
      expect(patients).toEqual(mockPatients);
    });

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('GET');
    req.flush(mockPatients);
  });

  it('should get all patients with pagination', () => {
    const mockResponse = {
      data: [{
        id: '1',
        firstName: 'John',
        lastName: 'Doe',
        birthDate: '1980-01-01',
        gender: 'Male',
        email: 'john.doe@example.com',
        passwordHash: 'hashedPassword',
        phoneNumber: '+1234567890',
        address: '123 Main St',
        patientRecords: []
      }],
      total: 1
    };

    service.getAll(1, 10).subscribe((response) => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${service['apiURL']}?page=1&pageSize=10`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should create a patient', () => {
    const newPatient: Patient = {
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1980-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      passwordHash: 'hashedPassword',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      patientRecords: []
    };

    service.createPatient(newPatient).subscribe((patient) => {
      expect(patient).toEqual(newPatient);
    });

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newPatient);
    req.flush(newPatient);
  });

  it('should update a patient', () => {
    const updatedPatient: Patient = {
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1980-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      passwordHash: 'hashedPassword',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      patientRecords: []
    };
    const token = 'fake-jwt-token';

    service.update('1', updatedPatient, token).subscribe((patient) => {
      expect(patient).toEqual(updatedPatient);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    expect(req.request.body).toEqual(updatedPatient);
    req.flush(updatedPatient);
  });

  it('should update patient password', () => {
    const passwordUpdate = { patientId: '1', password: 'newPassword123' };
    const token = 'fake-jwt-token';

    service.updatePassword(passwordUpdate, token).subscribe((response) => {
      expect(response).toBeUndefined();
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1/update-password`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    expect(req.request.body).toEqual(passwordUpdate);
    req.flush(null);  
  });

  it('should get patient by id', () => {
    const mockPatient: Patient = {
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1980-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      passwordHash: 'hashedPassword',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      patientRecords: []
    };
    const token = 'fake-jwt-token';

    service.getById('1', token).subscribe((patient) => {
      expect(patient).toEqual(mockPatient);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('GET');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    req.flush(mockPatient);
  });

  it('should get patient by id without token', () => {
    const mockPatient: Patient = {
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1980-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      passwordHash: 'hashedPassword',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      patientRecords: []
    };

    service.getByIdPatient('1').subscribe((patient) => {
      expect(patient).toEqual(mockPatient);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockPatient);
  });

  it('should delete a patient', () => {
    service.delete('1').subscribe((response) => {
      expect(response).toBeUndefined();
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null); // Respond with undefined
  });

  it('should check if email exists', () => {
    const email = 'test@example.com';
    const mockResponse = { exists: true };

    service.checkEmailExists(email).subscribe((exists) => {
      expect(exists).toBeTrue();
    });

    const req = httpMock.expectOne(`${service['apiURL']}/check-email?email=${email}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should logout', () => {
    spyOn(sessionStorage, 'removeItem');

    service.logout();

    expect(sessionStorage.removeItem).toHaveBeenCalledWith('jwtToken');
    expect(sessionStorage.removeItem).toHaveBeenCalledWith('userId');
    expect(sessionStorage.removeItem).toHaveBeenCalledWith('role');
    expect(routerMock.navigate).toHaveBeenCalledWith(['']);
  });

  it('should get all consultations', () => {
    const token = 'fake-jwt-token';
    const mockConsultations: Consultation[] = [{
      patientId: '1',
      medicId: '1',
      date: '2023-01-01',
      location: 'Room 101',
      status: ConsultationStatus.Pending
    }];

    service.getAllConsultations(token).subscribe((consultations) => {
      expect(consultations).toEqual(mockConsultations);
    });

    const req = httpMock.expectOne(service['apiUrl']);
    expect(req.request.method).toBe('GET');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    req.flush(mockConsultations);
  });

  it('should get medic by id', () => {
    const mockMedic = { id: '1', firstName: 'Jane', lastName: 'Doe' };

    service.getMedicById('1').subscribe((medic) => {
      expect(medic).toEqual(mockMedic);
    });

    const req = httpMock.expectOne(`${service['medicUrl']}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockMedic);
  });
});