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

  private apiURL = 'http://localhost:5210/api/v1/Consultations/request'; // URL for the consultations API

  constructor(private http: HttpClient, private router: Router) { }

  createConsultation(consultation: Consultation): Observable<Consultation> {
    const headers = new HttpHeaders({});
    return this.http.post<Consultation>(this.apiURL, consultation, { headers }).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    console.error('An error occurred:', error.message);
    return throwError('Something bad happened; please try again later.');
  }
}