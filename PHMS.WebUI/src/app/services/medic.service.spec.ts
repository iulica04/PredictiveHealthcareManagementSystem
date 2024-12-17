import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { MedicService } from './medic.service';
import { Medic } from '../models/medic.model';
import { Router } from '@angular/router';

fdescribe('MedicService', () => {
  let service: MedicService;
  let httpMock: HttpTestingController;
  let router: Router;
  const token = 'fake-jwt-token';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [MedicService, { provide: Router, useValue: { navigate: jasmine.createSpy('navigate') } }]
    });
    service = TestBed.inject(MedicService);
    httpMock = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should retrieve all medics', () => {
    const mockMedics: Medic[] = [
      { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', rank: 'Senior', specialization: 'Cardiology', hospital: 'General Hospital', passwordHash: 'hashedpassword' },
      { id: '2', firstName: 'Jane', lastName: 'Smith', birthDate: '1990-02-02', gender: 'Female', email: 'jane.smith@example.com', phoneNumber: '+1234567891', address: '456 Main St', rank: 'Junior', specialization: 'Neurology', hospital: 'City Hospital', passwordHash: 'hashedpassword' }
    ];

    service.getMedics().subscribe((medics) => {
      expect(medics).toEqual(mockMedics);
    });

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('GET');
    req.flush(mockMedics);
  });

  it('should retrieve medics with pagination', () => {
    const mockResponse = {
      data: [
        { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', rank: 'Senior', specialization: 'Cardiology', hospital: 'General Hospital', passwordHash: 'hashedpassword' },
        { id: '2', firstName: 'Jane', lastName: 'Smith', birthDate: '1990-02-02', gender: 'Female', email: 'jane.smith@example.com', phoneNumber: '+1234567891', address: '456 Main St', rank: 'Junior', specialization: 'Neurology', hospital: 'City Hospital', passwordHash: 'hashedpassword' }
      ],
      totalCount: 2
    };

    service.getAll(1, 10).subscribe((response) => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/paginated?page=1&pageSize=10`);
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should create a new medic', () => {
    const newMedic: Medic = { id: '3', firstName: 'Alice', lastName: 'Johnson', birthDate: '1985-05-05', gender: 'Female', email: 'alice.johnson@example.com', phoneNumber: '+1234567892', address: '789 Main St', rank: 'Junior', specialization: 'Pediatrics', hospital: 'Children Hospital', passwordHash: 'hashedpassword' };

    service.createMedic(newMedic).subscribe((medic) => {
      expect(medic).toEqual(newMedic);
    });

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newMedic);
    req.flush(newMedic);
  });

  it('should update a medic', () => {
    const updatedMedic: Medic = { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', rank: 'Senior', specialization: 'Cardiology', hospital: 'General Hospital', passwordHash: 'hashedpassword' };

    service.update('1', updatedMedic, token).subscribe((medic) => {
      expect(medic).toEqual(updatedMedic);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    expect(req.request.body).toEqual(updatedMedic);
    req.flush(updatedMedic);
  });

  it('should retrieve a medic by id', () => {
    const mockMedic: Medic = { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', rank: 'Senior', specialization: 'Cardiology', hospital: 'General Hospital', passwordHash: 'hashedpassword' };

    service.getById('1').subscribe((medic) => {
      expect(medic).toEqual(mockMedic);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockMedic);
  });

  it('should delete a medic', () => {
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

  it('should handle error when updating a medic', () => {
    const updatedMedic: Medic = { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', rank: 'Senior', specialization: 'Cardiology', hospital: 'General Hospital', passwordHash: 'hashedpassword' };

    service.update('1', updatedMedic, token).subscribe(
      () => fail('should have failed with the 500 error'),
      (error) => {
        expect(error.status).toBe(500);
      }
    );

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('PUT');
    req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
  });

  it('should handle error when retrieving a medic by id', () => {
    service.getById('1').subscribe(
      () => fail('should have failed with the 500 error'),
      (error) => {
        expect(error.status).toBe(500);
      }
    );

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('GET');
    req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
  });

  it('should handle error when deleting a medic', () => {
    service.delete('1', token).subscribe(
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