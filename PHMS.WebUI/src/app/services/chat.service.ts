import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DiseasePredictionService {
  private apiUrl = 'http://localhost:5210/api/v1/HealthChatbot/predict'; // Backend API URL

  constructor(private http: HttpClient) { }

  predictDisease(symptoms: string): Observable<any> {
    return this.http.post<any>(this.apiUrl, { symptoms });
  }
}
