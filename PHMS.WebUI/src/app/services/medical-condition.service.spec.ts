import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { MedicalConditionService } from './medical-condition.service';
import { MedicalCondition } from '../models/medicalCondition.model';

fdescribe('MedicalConditionService', () => {
  let service: MedicalConditionService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [MedicalConditionService]
    });
    service = TestBed.inject(MedicalConditionService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should retrieve medical conditions by patient ID', () => {
    const dummyConditions: MedicalCondition[] = [
      { medicalConditionId: '1', name: 'Condition A', description: 'Description A', patientId: '123', startDate: '2022-01-01', currentStatus: 'Active', recommendation: '', treatments: [] },
      { medicalConditionId: '2', name: 'Condition B', description: 'Description B', patientId: '123', startDate: '2022-01-01', currentStatus: 'Active', recommendation: '', treatments: [] }
    ];

    service.getMedicalConditionsByPatientId('123').subscribe(conditions => {
      expect(conditions.length).toBe(2);
      expect(conditions).toEqual(dummyConditions);
    });

    const req = httpMock.expectOne(`http://localhost:5210/api/v1/MedicalCondition/patient/123`);
    expect(req.request.method).toBe('GET');
    req.flush(dummyConditions);
  });

  it('should create a new medical condition', () => {
    const newCondition: MedicalCondition = { medicalConditionId: '3', name: 'Condition C', description: 'Description C', patientId: '123', startDate: '2022-01-01', currentStatus: 'Active', recommendation: '', treatments: [] };

    service.createMedicalCondition(newCondition).subscribe(response => {
      expect(response).toEqual(newCondition);
    });

    const req = httpMock.expectOne('http://localhost:5210/api/v1/MedicalCondition');
    expect(req.request.method).toBe('POST');
    req.flush(newCondition);
  });
});
