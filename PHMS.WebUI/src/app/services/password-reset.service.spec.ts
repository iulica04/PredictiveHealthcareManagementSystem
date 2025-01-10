import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PasswordResetService } from './password-reset.service';

fdescribe('PasswordResetService', () => {
  let service: PasswordResetService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PasswordResetService]
    });
    service = TestBed.inject(PasswordResetService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should send reset link and return response', () => {
    const email = 'test@example.com';
    const mockResponse = { success: true, message: 'Reset link sent' };

    service.sendResetLink(email).subscribe((response) => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${service['apiUrl']}/forgot-password?email=${email}`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toBeNull();
    req.flush(mockResponse);
  });

  it('should reset password and return response', () => {
    const email = 'test@example.com';
    const token = 'reset-token';
    const newPassword = 'new-password';
    const mockResponse = { success: true, message: 'Password reset successful' };

    service.resetPassword(email, token, newPassword).subscribe((response) => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${service['apiUrl']}/reset-password`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ email, token, newPassword });
    req.flush(mockResponse);
  });

  it('should handle error response for sendResetLink', () => {
    const email = 'test@example.com';

    service.sendResetLink(email).subscribe(
      () => fail('should have failed with the 500 error'),
      (error) => {
        expect(error.status).toBe(500);
      }
    );

    const req = httpMock.expectOne(`${service['apiUrl']}/forgot-password?email=${email}`);
    expect(req.request.method).toBe('POST');
    req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
  });

  it('should handle error response for resetPassword', () => {
    const email = 'test@example.com';
    const token = 'reset-token';
    const newPassword = 'new-password';

    service.resetPassword(email, token, newPassword).subscribe(
      () => fail('should have failed with the 500 error'),
      (error) => {
        expect(error.status).toBe(500);
      }
    );

    const req = httpMock.expectOne(`${service['apiUrl']}/reset-password`);
    expect(req.request.method).toBe('POST');
    req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
  });
});