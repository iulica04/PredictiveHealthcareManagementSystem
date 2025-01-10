import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { EmailVerificationService } from './email-verification.service';

fdescribe('EmailVerificationService', () => {
  let service: EmailVerificationService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [EmailVerificationService]
    });
    service = TestBed.inject(EmailVerificationService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return true if email exists', () => {
    const email = 'test@example.com';
    service.checkEmailExists(email).subscribe((exists) => {
      expect(exists).toBeTrue();
    });

    const req = httpMock.expectOne(`${service['apiURL']}/check-email?email=${email}`);
    expect(req.request.method).toBe('GET');
    req.flush({ exists: true });
  });

  it('should return false if email does not exist', () => {
    const email = 'test@example.com';
    service.checkEmailExists(email).subscribe((exists) => {
      expect(exists).toBeFalse();
    });

    const req = httpMock.expectOne(`${service['apiURL']}/check-email?email=${email}`);
    expect(req.request.method).toBe('GET');
    req.flush({ exists: false });
  });

  it('should handle error response', () => {
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