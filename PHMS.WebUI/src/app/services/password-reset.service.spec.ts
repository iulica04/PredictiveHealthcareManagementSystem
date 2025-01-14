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

  it('should send reset link', () => {
    const email = 'test@example.com';
    const mockResponse = { success: true, message: 'Password reset link sent' };

    service.sendResetLink(email).subscribe((response) => {
      expect(response.success).toBeTrue();
      expect(response.message).toBe('Password reset link sent');
    });

    const req = httpMock.expectOne(`${service['apiUrl']}/forgot-password?email=${email}`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toBeNull();
    req.flush(mockResponse);
  });

  it('should reset password', () => {
    const email = 'test@example.com';
    const token = 'reset-token';
    const newPassword = 'newPassword123';
    const mockResponse = { success: true, message: 'Password has been reset' };

    service.resetPassword(email, token, newPassword).subscribe((response) => {
      expect(response.success).toBeTrue();
      expect(response.message).toBe('Password has been reset');
    });

    const req = httpMock.expectOne(`${service['apiUrl']}/reset-password`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ email, token, newPassword });
    req.flush(mockResponse);
  });
});