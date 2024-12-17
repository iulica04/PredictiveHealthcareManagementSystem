import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PatientService } from './patient.service';
import { Patient } from '../models/patient.model';

fdescribe('PatientService', () => {
  let service: PatientService;
  let httpMock: HttpTestingController;
  const token = 'test-token';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PatientService]
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

  it('should retrieve all patients', () => {
    const dummyPatients: Patient[] = [
      { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', passwordHash: 'hashedpassword', patientRecords: [] },
      { id: '2', firstName: 'Jane', lastName: 'Smith', birthDate: '1995-02-10', gender: 'Female', email: 'jane.smith@example.com', phoneNumber: '+0987654321', address: '456 Oak St', passwordHash: 'hashedpassword', patientRecords: [] }
    ];

    service.getPatients().subscribe(patients => {
      expect(patients.length).toBe(2);
      expect(patients).toEqual(dummyPatients);
    });

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('GET');
    req.flush(dummyPatients);
  });

  it('should retrieve patients with pagination', () => {
    const dummyPatients: Patient[] = [
      { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', passwordHash: 'hashedpassword', patientRecords: [] }
    ];

    service.getAll(1, 10).subscribe(response => {
      expect(response.data.length).toBe(1);
      expect(response.data).toEqual(dummyPatients);
    });

    const req = httpMock.expectOne(`${service['apiURL']}?page=1&pageSize=10`);
    expect(req.request.method).toBe('GET');
    req.flush({ data: dummyPatients, totalCount: 1 });
  });

  it('should create a new patient', () => {
    const newPatient: Patient = { id: '3', firstName: 'Alice', lastName: 'Johnson', birthDate: '1985-05-15', gender: 'Female', email: 'alice.johnson@example.com', phoneNumber: '+1122334455', address: '789 Pine St', passwordHash: 'hashedpassword', patientRecords: [] };

    service.createPatient(newPatient).subscribe(patient => {
      expect(patient).toEqual(newPatient);
    });

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('POST');
    req.flush(newPatient);
  });

  it('should update an existing patient', () => {
    const updatedPatient: Patient = { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', passwordHash: 'hashedpassword', patientRecords: [] };

    service.update('1', updatedPatient, token).subscribe(patient => {
      expect(patient).toEqual(updatedPatient);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('PUT');
    req.flush(updatedPatient);
  });

  it('should retrieve a patient by id', () => {
    const dummyPatient: Patient = { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', passwordHash: 'hashedpassword', patientRecords: [] };

    service.getById('1', token).subscribe(patient => {
      expect(patient).toEqual(dummyPatient);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(dummyPatient);
  });

 

  it('should handle error when retrieving all patients', () => {
    service.getPatients().subscribe(
      () => fail('should have failed with the 500 error'),
      (error) => {
        expect(error.status).toBe(500);
      }
    );

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('GET');
    req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
  });

  it('should handle error when retrieving patients with pagination', () => {
    service.getAll(1, 10).subscribe(
      () => fail('should have failed with the 500 error'),
      (error) => {
        expect(error.status).toBe(500);
      }
    );

    const req = httpMock.expectOne(`${service['apiURL']}?page=1&pageSize=10`);
    expect(req.request.method).toBe('GET');
    req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
  });

  it('should handle error when creating a new patient', () => {
    const newPatient: Patient = { id: '3', firstName: 'Alice', lastName: 'Johnson', birthDate: '1985-05-15', gender: 'Female', email: 'alice.johnson@example.com', phoneNumber: '+1122334455', address: '789 Pine St', passwordHash: 'hashedpassword', patientRecords: [] };

    service.createPatient(newPatient).subscribe(
      () => fail('should have failed with the 500 error'),
      (error) => {
        expect(error.status).toBe(500);
      }
    );

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('POST');
    req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
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

  it('should handle error when deleting a patient by id', () => {
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
});