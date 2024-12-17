import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PatientService } from './patient.service';
import { Patient } from '../models/patient.model';
import { Router } from '@angular/router';

fdescribe('PatientService', () => {
  let service: PatientService;
  let httpMock: HttpTestingController;
  let router: Router;
  const token = 'fake-jwt-token';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PatientService, { provide: Router, useValue: { navigate: jasmine.createSpy('navigate') } }]
    });
    service = TestBed.inject(PatientService);
    httpMock = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should retrieve all patients', () => {
    const mockPatients: Patient[] = [
      { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', passwordHash: 'hashedpassword', patientRecords: [] },
      { id: '2', firstName: 'Jane', lastName: 'Smith', birthDate: '1990-02-02', gender: 'Female', email: 'jane.smith@example.com', phoneNumber: '+1234567891', address: '456 Main St', passwordHash: 'hashedpassword', patientRecords: [] }
    ];

    service.getPatients().subscribe((patients) => {
      expect(patients).toEqual(mockPatients);
    });

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('GET');
    req.flush(mockPatients);
  });

  it('should retrieve patients with pagination', () => {
    const mockResponse = {
      data: [
        { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', passwordHash: 'hashedpassword', patientRecords: [] },
        { id: '2', firstName: 'Jane', lastName: 'Smith', birthDate: '1990-02-02', gender: 'Female', email: 'jane.smith@example.com', phoneNumber: '+1234567891', address: '456 Main St', passwordHash: 'hashedpassword', patientRecords: [] }
      ],
      totalCount: 2
    };

    service.getAll(1, 10).subscribe((response) => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${service['apiURL']}?page=1&pageSize=10`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should create a new patient', () => {
    const newPatient: Patient = { id: '3', firstName: 'Alice', lastName: 'Johnson', birthDate: '1985-05-05', gender: 'Female', email: 'alice.johnson@example.com', phoneNumber: '+1234567892', address: '789 Main St', passwordHash: 'hashedpassword', patientRecords: [] };

    service.createPatient(newPatient).subscribe((patient) => {
      expect(patient).toEqual(newPatient);
    });

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newPatient);
    req.flush(newPatient);
  });

  it('should update a patient', () => {
    const updatedPatient: Patient = { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', passwordHash: 'hashedpassword', patientRecords: [] };

    service.update('1', updatedPatient, token).subscribe((patient) => {
      expect(patient).toEqual(updatedPatient);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    expect(req.request.body).toEqual(updatedPatient);
    req.flush(updatedPatient);
  });

  it('should retrieve a patient by id', () => {
    const mockPatient: Patient = { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', passwordHash: 'hashedpassword', patientRecords: [] };

    service.getById('1', token).subscribe((patient) => {
      expect(patient).toEqual(mockPatient);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('GET');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    req.flush(mockPatient);
  });

  it('should delete a patient', () => {
    service.delete('1').subscribe((response) => {
      expect(response).toBeUndefined();
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({});
  });

  it('should check if email exists', () => {
    const email = 'test@example.com';
    service.checkEmailExists(email).subscribe((exists) => {
      expect(exists).toBeTrue();
    });

    const req = httpMock.expectOne(`${service['apiURL']}/check-email?email=${email}`);
    expect(req.request.method).toBe('GET');
    req.flush({ exists: true });
  });

  it('should logout and clear session storage', () => {
    spyOn(sessionStorage, 'removeItem');
    service.logout();
    expect(sessionStorage.removeItem).toHaveBeenCalledWith('jwtToken');
    expect(sessionStorage.removeItem).toHaveBeenCalledWith('userId');
    expect(sessionStorage.removeItem).toHaveBeenCalledWith('role');
    expect(router.navigate).toHaveBeenCalledWith(['']);
  });

  it('should handle error when updating a patient', () => {
    const updatedPatient: Patient = { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', passwordHash: 'hashedpassword', patientRecords: [] };

    service.update('1', updatedPatient, token).subscribe(
      () => fail('should have failed with the 500 error'),
      (error) => {
        expect(error.status).toBe(500);
      }
    );

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('PUT');
    req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
  });

  it('should handle error when retrieving a patient by id', () => {
    service.getById('1', token).subscribe(
      () => fail('should have failed with the 500 error'),
      (error) => {
        expect(error.status).toBe(500);
      }
    );

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('GET');
    req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
  });

  it('should handle error when deleting a patient', () => {
    service.delete('1').subscribe(
      () => fail('should have failed with the 500 error'),
      (error) => {
        expect(error.status).toBe(500);
      }
    );

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
  });

  it('should handle error when checking if email exists', () => {
    const email = 'test@example.com';
    service.checkEmailExists(email).subscribe(
      () => fail('should have failed with the 500 error'),
      (error) => {
        expect(error.status).toBe(500);
      }
    );

    const req = httpMock.expectOne(`${service['apiURL']}/check-email?email=${email}`);
    expect(req.request.method).toBe('GET');
    req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
  });
});