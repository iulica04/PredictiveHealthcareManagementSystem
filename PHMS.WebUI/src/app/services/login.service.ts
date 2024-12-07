import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  private apiUrl = 'https://api.example.com'; // Înlocuiește cu URL-ul API-ului tău

  constructor(private http: HttpClient) {}

  login(credentials: { email: string; password: string }): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        // Salvează token-ul de autentificare în localStorage sau sessionStorage
        localStorage.setItem('authToken', response.token);
      })
    );
  }

  logout(): void {
    // Șterge token-ul de autentificare din localStorage sau sessionStorage
    localStorage.removeItem('authToken');
  }

  isLoggedIn(): boolean {
    // Verifică dacă există un token de autentificare în localStorage sau sessionStorage
    return !!localStorage.getItem('authToken');
  }
}