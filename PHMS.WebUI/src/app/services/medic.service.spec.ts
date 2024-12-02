import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { MedicService } from './medic.service';
import { Medic } from '../models/medic.model';

fdescribe('MedicService', () => {
  let service: MedicService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [MedicService]
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

  it('should retrieve all medics', () => {
    const dummyMedics: Medic[] = [
      { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', rank: 'Senior', specialization: 'Cardiology', hospital: 'General Hospital', passwordHash: 'hashedpassword' },
      { id: '2', firstName: 'Jane', lastName: 'Smith', birthDate: '1995-02-10', gender: 'Female', email: 'jane.smith@example.com', phoneNumber: '+0987654321', address: '456 Oak St', rank: 'Junior', specialization: 'Neurology', hospital: 'City Hospital', passwordHash: 'hashedpassword' }
    ];

    service.getMedics().subscribe(medics => {
      expect(medics.length).toBe(2);
      expect(medics).toEqual(dummyMedics);
    });

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('GET');
    req.flush(dummyMedics);
  });

  it('should retrieve medics with pagination', () => {
    const dummyMedics: Medic[] = [
      { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', rank: 'Senior', specialization: 'Cardiology', hospital: 'General Hospital', passwordHash: 'hashedpassword' }
    ];

    service.getAll(1, 10, 'Senior', 'Cardiology').subscribe(medics => {
      expect(medics.length).toBe(1);
      expect(medics).toEqual(dummyMedics);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/paginated?page=1&pageSize=10&rank=Senior&specialization=Cardiology`);
    expect(req.request.method).toBe('GET');
    req.flush(dummyMedics);
  });

  it('should create a new medic', () => {
    const newMedic: Medic = { id: '3', firstName: 'Alice', lastName: 'Johnson', birthDate: '1985-05-15', gender: 'Female', email: 'alice.johnson@example.com', phoneNumber: '+1122334455', address: '789 Pine St', rank: 'Senior', specialization: 'Dermatology', hospital: 'Regional Hospital', passwordHash: 'hashedpassword' };

    service.createMedic(newMedic).subscribe(medic => {
      expect(medic).toEqual(newMedic);
    });

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('POST');
    req.flush(newMedic);
  });

  it('should update an existing medic', () => {
    const updatedMedic: Medic = { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', rank: 'Senior', specialization: 'Cardiology', hospital: 'General Hospital', passwordHash: 'hashedpassword' };

    service.update('1', updatedMedic).subscribe(medic => {
      expect(medic).toEqual(updatedMedic);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('PUT');
    req.flush(updatedMedic);
  });

  it('should retrieve a medic by id', () => {
    const dummyMedic: Medic = { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', rank: 'Senior', specialization: 'Cardiology', hospital: 'General Hospital', passwordHash: 'hashedpassword' };

    service.getById('1').subscribe(medic => {
      expect(medic).toEqual(dummyMedic);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(dummyMedic);
  });


  it('should handle error when retrieving all medics', () => {
    service.getMedics().subscribe(
      () => fail('should have failed with the 500 error'),
      (error) => {
        expect(error.status).toBe(500);
      }
    );

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('GET');
    req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
  });

  it('should handle error when retrieving medics with pagination', () => {
    service.getAll(1, 10, 'Senior', 'Cardiology').subscribe(
      () => fail('should have failed with the 500 error'),
      (error) => {
        expect(error.status).toBe(500);
      }
    );

    const req = httpMock.expectOne(`${service['apiURL']}/paginated?page=1&pageSize=10&rank=Senior&specialization=Cardiology`);
    expect(req.request.method).toBe('GET');
    req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
  });

  it('should handle error when creating a new medic', () => {
    const newMedic: Medic = { id: '3', firstName: 'Alice', lastName: 'Johnson', birthDate: '1985-05-15', gender: 'Female', email: 'alice.johnson@example.com', phoneNumber: '+1122334455', address: '789 Pine St', rank: 'Senior', specialization: 'Dermatology', hospital: 'Regional Hospital', passwordHash: 'hashedpassword' };

    service.createMedic(newMedic).subscribe(
      () => fail('should have failed with the 500 error'),
      (error) => {
        expect(error.status).toBe(500);
      }
    );

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('POST');
    req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
  });

  it('should handle error when updating a medic', () => {
    const updatedMedic: Medic = { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', rank: 'Senior', specialization: 'Cardiology', hospital: 'General Hospital', passwordHash: 'hashedpassword' };

    service.update('1', updatedMedic).subscribe(
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

  it('should handle error when deleting a medic by id', () => {
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