import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private apiUrl = 'http://localhost:5210/api/Chatbot/get-response'; // Backend API URL

  constructor(private http: HttpClient) { }

  getResponse(Input: string): Observable<any> {
    // Trimite "Input" pentru a se potrivi cu modelul așteptat de backend
    return this.http.post<any>(this.apiUrl, { Input: Input });
  }
  
}
