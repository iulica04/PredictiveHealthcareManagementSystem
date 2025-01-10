import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { Router } from '@angular/router';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiURL ='http://localhost:5210/api/v1/Auth';

  constructor(private http: HttpClient, private router: Router) {}

  login(credentials: { email: string, password: string }): Observable<any> {
    return this.http.post<any>(`${this.apiURL}/login`, credentials);
  }

  logout(): void {
    sessionStorage.removeItem('jwtToken');
    sessionStorage.removeItem('userId');
    sessionStorage.removeItem('role');
    this.router.navigate(['']);
  }

  checkEmailExists(email: string): Observable<boolean> {
    return this.http.get<{ exists: boolean }>(`${this.apiURL}/check-email?email=${email}`).pipe(
      map((response: any) => response.exists)
    );
  }

  register(user: User): Observable<any> {
    return this.http.post<any>(`${this.apiURL}/register`, user);
  }

  sendResetLink(email: string): Observable<{ success: boolean, message: string }> {
    return this.http.post<{ success: boolean, message: string }>(`${this.apiURL}/forgot-password?email=${email}`, null);
  }

  resetPassword(email:string, token: string, newPassword: string): Observable<{ success: boolean, message: string }> {
    return this.http.post<{ success: boolean, message: string }>(`${this.apiURL}/reset-password`, {email, token, newPassword });
  }
}
