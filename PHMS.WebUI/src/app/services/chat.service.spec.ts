import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ChatService } from './chat.service';

fdescribe('ChatService', () => {
  let service: ChatService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ChatService]
    });
    service = TestBed.inject(ChatService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call getResponse and return an Observable', () => {
    const mockResponse = { message: 'Hello, how can I assist you today?' };
    const input = 'Hello';

    service.getResponse(input).subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne('http://localhost:5210/api/Chatbot/get-response');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ Input: input });
    
    req.flush(mockResponse); // Mock the response
  });

  afterEach(() => {
    httpMock.verify(); // Verify no outstanding HTTP requests
  });
});
