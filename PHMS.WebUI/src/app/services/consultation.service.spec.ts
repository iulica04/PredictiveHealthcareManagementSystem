import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ConsultationService } from './consultation.service';
import { Consultation, ConsultationStatus } from '../models/consultation.model'; // Ensure you have defined the Consultation model
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';

fdescribe('ConsultationService', () => {
  let service: ConsultationService;
  let httpMock: HttpTestingController;
  let routerMock: any;

  beforeEach(() => {
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ConsultationService, { provide: Router, useValue: routerMock }]
    });

    service = TestBed.inject(ConsultationService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should create a consultation', () => {
    const newConsultation: Consultation = {
      patientId: '1',
      medicId: '1',
      date: '2023-01-01',
      location: 'Room 101',
      status: ConsultationStatus.Pending
    };

    service.createConsultation(newConsultation).subscribe((consultation) => {
      expect(consultation).toEqual(newConsultation);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/request`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newConsultation);
    req.flush(newConsultation);
  });

  it('should handle error when creating consultation', () => {
    const errorMessage = 'Http failure response for http://localhost:5210/api/v1/Consultation/request: 500 Server Error';
    const newConsultation: Consultation = {
      patientId: '1',
      medicId: '1',
      date: '2023-01-01',
      location: 'Room 101',
      status: ConsultationStatus.Pending
    };

    service.createConsultation(newConsultation).subscribe({
      next: () => fail('expected an error, not consultation'),
      error: (error: HttpErrorResponse) => {
        expect(error.status).toBe(500);
        expect(error.message).toContain('Server Error');
      }
    });

    const req = httpMock.expectOne(`${service['apiURL']}/request`);
    expect(req.request.method).toBe('POST');
    req.flush({ message: errorMessage }, { status: 500, statusText: 'Server Error' });
  });

  it('should get consultation by id', () => {
    const mockConsultation: Consultation = {
      patientId: '1',
      medicId: '1',
      date: '2023-01-01',
      location: 'Room 101',
      status: ConsultationStatus.Pending
    };

    service.getAppointmentById('1').subscribe((consultation) => {
      expect(consultation).toEqual(mockConsultation);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockConsultation);
  });

  it('should handle error when getting consultation by id', () => {
    const errorMessage = 'Http failure response for http://localhost:5210/api/v1/Consultation/1: 500 Server Error';

    service.getAppointmentById('1').subscribe({
      next: () => fail('expected an error, not consultation'),
      error: (error: HttpErrorResponse) => {
        expect(error.status).toBe(500);
        expect(error.message).toContain('Server Error');
      }
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('GET');
    req.flush({ message: errorMessage }, { status: 500, statusText: 'Server Error' });
  });

  it('should update a consultation', () => {
    const updatedConsultation: Consultation = {
      patientId: '1',
      medicId: '1',
      date: '2023-01-01',
      location: 'Room 101',
      status: ConsultationStatus.Accepted
    };

    service.updateConsultation('1', updatedConsultation).subscribe((consultation) => {
      expect(consultation).toEqual(updatedConsultation);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updatedConsultation);
    req.flush(updatedConsultation);
  });

  it('should handle error when updating consultation', () => {
    const errorMessage = 'Http failure response for http://localhost:5210/api/v1/Consultation/1: 500 Server Error';
    const updatedConsultation: Consultation = {
      patientId: '1',
      medicId: '1',
      date: '2023-01-01',
      location: 'Room 101',
      status: ConsultationStatus.Accepted
    };

    service.updateConsultation('1', updatedConsultation).subscribe({
      next: () => fail('expected an error, not consultation'),
      error: (error: HttpErrorResponse) => {
        expect(error.status).toBe(500);
        expect(error.message).toContain('Server Error');
      }
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('PUT');
    req.flush({ message: errorMessage }, { status: 500, statusText: 'Server Error' });
  });

  it('should delete a consultation', () => {
    service.deleteConsultation('1').subscribe((response) => {
      expect(response).toBeUndefined();
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null); 
  });

  it('should handle error when deleting consultation', () => {
    const errorMessage = 'Http failure response for http://localhost:5210/api/v1/Consultation/1: 500 Server Error';

    service.deleteConsultation('1').subscribe({
      next: () => fail('expected an error, not void'),
      error: (error: HttpErrorResponse) => {
        expect(error.status).toBe(500);
        expect(error.message).toContain('Server Error');
      }
    });

    const req = httpMock.expectOne(`${service['apiURL']}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ message: errorMessage }, { status: 500, statusText: 'Server Error' });
  });
});