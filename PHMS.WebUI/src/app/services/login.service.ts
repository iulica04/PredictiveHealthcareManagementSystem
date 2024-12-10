import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  private apiURL ='http://localhost:5210/api/v1/Medic/login';

  constructor(private http: HttpClient) {}

  login(credentials: { email: string, password: string }): Observable<any> {
    return this.http.post<any>(this.apiURL, credentials);  // Înlocuiește cu endpoint-ul tău
  }
}
