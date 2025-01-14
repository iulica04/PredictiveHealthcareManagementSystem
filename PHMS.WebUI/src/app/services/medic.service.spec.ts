import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { MedicService } from './medic.service';
import { Medic } from '../models/medic.model';
import { Consultation, ConsultationStatus } from '../models/consultation.model';
import { Router } from '@angular/router';

fdescribe('MedicService', () => {
  let service: MedicService;
  let httpMock: HttpTestingController;
  let routerMock: any;

  beforeEach(() => {
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [MedicService, { provide: Router, useValue: routerMock }]
    });

    service = TestBed.inject(MedicService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get medics', () => {
    const mockMedics: Medic[] = [{
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1980-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      passwordHash: 'hashedPassword',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
    }];

    service.getMedics().subscribe((medics) => {
      expect(medics).toEqual(mockMedics);
    });

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('GET');
    req.flush(mockMedics);
  });

  it('should get all medics with pagination', () => {
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
        rank: 'Senior',
        specialization: 'Cardiology',
        hospital: 'General Hospital'
      }],
      total: 1
    };

    service.getAll(1, 10).subscribe((response) => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/paginated?page=1&pageSize=10`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should create a medic', () => {
    const newMedic: Medic = {
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1980-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      passwordHash: 'hashedPassword',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
    };

    service.createMedic(newMedic).subscribe((medic) => {
      expect(medic).toEqual(newMedic);
    });

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newMedic);
    req.flush(newMedic);
  });

  it('should update a medic', () => {
    const updatedMedic: Medic = {
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1980-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      passwordHash: 'hashedPassword',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
    };
    const token = 'fake-jwt-token';

    service.update('1', updatedMedic, token).subscribe((medic) => {
      expect(medic).toEqual(updatedMedic);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    expect(req.request.body).toEqual(updatedMedic);
    req.flush(updatedMedic);
  });

  it('should get medic by id', () => {
    const mockMedic: Medic = {
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1980-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      passwordHash: 'hashedPassword',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
    };

    service.getById('1').subscribe((medic) => {
      expect(medic).toEqual(mockMedic);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockMedic);
  });

  it('should delete a medic', () => {
    const token = 'fake-jwt-token';

    service.delete('1', token).subscribe((response) => {
      expect(response).toBeUndefined();
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('DELETE');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    req.flush({});
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

  it('should get patient by id', () => {
    const mockPatient = { id: '1', firstName: 'John', lastName: 'Doe' };

    service.getPatientById('1').subscribe((patient) => {
      expect(patient).toEqual(mockPatient);
    });

    const req = httpMock.expectOne(`${service['patientUrl']}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockPatient);
  });

  // Test pentru acoperirea ramurilor condiționale în metoda getAll
it('should get all medics with pagination and filters', () => {
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
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
    }],
    total: 1
  };

  service.getAll(1, 10, 'Senior', 'Cardiology').subscribe((response) => {
    expect(response).toEqual(mockResponse);
  });

  const req = httpMock.expectOne(`${service['apiURL']}/paginated?page=1&pageSize=10&rank=Senior&specialization=Cardiology`);
  expect(req.request.method).toBe('GET');
  req.flush(mockResponse);
});

// Test pentru cazurile în care metoda getAll nu primește rank sau specialization
it('should get all medics with pagination without filters', () => {
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
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
    }],
    total: 1
  };

  service.getAll(1, 10).subscribe((response) => {
    expect(response).toEqual(mockResponse);
  });

  const req = httpMock.expectOne(`${service['apiURL']}/paginated?page=1&pageSize=10`);
  expect(req.request.method).toBe('GET');
  req.flush(mockResponse);
});

// Test pentru excepții în metoda getMedics
it('should handle error when getting medics', () => {
  const errorMessage = 'Http failure response for http://localhost:5210/api/v1/Medic: 500 Server Error';

  service.getMedics().subscribe({
    next: () => fail('expected an error, not medics'),
    error: (error) => expect(error.message).toContain('Http failure response for http://localhost:5210/api/v1/Medic: 500 Server Error')
  });

  const req = httpMock.expectOne(service['apiURL']);
  expect(req.request.method).toBe('GET');
  req.flush({ message: errorMessage }, { status: 500, statusText: 'Server Error' });
});

// Test pentru excepții în metoda createMedic
it('should handle error when creating medic', () => {
  const errorMessage = 'Http failure response for http://localhost:5210/api/v1/Medic: 500 Server Error';
  const newMedic: Medic = {
    id: '1',
    firstName: 'John',
    lastName: 'Doe',
    birthDate: '1980-01-01',
    gender: 'Male',
    email: 'john.doe@example.com',
    passwordHash: 'hashedPassword',
    phoneNumber: '+1234567890',
    address: '123 Main St',
    rank: 'Senior',
    specialization: 'Cardiology',
    hospital: 'General Hospital'
  };

  service.createMedic(newMedic).subscribe({
    next: () => fail('expected an error, not medic'),
    error: (error) => expect(error.message).toContain('Http failure response for http://localhost:5210/api/v1/Medic: 500 Server Error')
  });

  const req = httpMock.expectOne(service['apiURL']);
  expect(req.request.method).toBe('POST');
  req.flush({ message: errorMessage }, { status: 500, statusText: 'Server Error' });
});

});