import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { Consultation } from '../models/consultation.model';  // Ensure you have defined the Consultation model
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class ConsultationService {

  private apiURL = 'http://localhost:5210/api/v1/Consultation'; // URL for the consultations API

  constructor(private http: HttpClient, private router: Router) { }

  createConsultation(consultation: Consultation): Observable<Consultation> {
    return this.http.post<Consultation>(`${this.apiURL}/request`, consultation);
  }

  private handleError(error: HttpErrorResponse) {
    console.error('An error occurred:', error.message);
    return throwError('Something bad happened; please try again later.');
  }

  getAppointmentById(id: string): Observable<Consultation> {
    return this.http.get<Consultation>(`${this.apiURL}/${id}`);
  }
  updateConsultation(id: string, appointment: Consultation): Observable<Consultation> {
    return this.http.put<Consultation>(`${this.apiURL}/${id}`, appointment).pipe(
      catchError(this.handleError)
    );
  }
  deleteConsultation(appointmentId: string) {
    const url = `${this.apiURL}/${appointmentId}`;
    return this.http.delete(url, {
 });
  }
  
}