import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { LoginService } from './login.service';

fdescribe('LoginService', () => {
  let service: LoginService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [LoginService]
    });

    service = TestBed.inject(LoginService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should login successfully', () => {
    const credentials = { email: 'test@example.com', password: 'password123' };
    const mockResponse = { token: 'fake-jwt-token' };

    service.login(credentials).subscribe((response) => {
      expect(response.token).toBe('fake-jwt-token');
    });

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(credentials);
    req.flush(mockResponse);
  });

  it('should handle login error', () => {
    const credentials = { email: 'test@example.com', password: 'password123' };
    const mockError = { status: 401, statusText: 'Unauthorized' };

    service.login(credentials).subscribe(
      () => fail('should have failed with 401 error'),
      (error) => {
        expect(error.status).toBe(401);
        expect(error.statusText).toBe('Unauthorized');
      }
    );

    const req = httpMock.expectOne(service['apiURL']);
    expect(req.request.method).toBe('POST');
    req.flush(null, mockError);
  });
});