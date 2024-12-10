import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Patient } from '../models/patient.model';

@Injectable({
  providedIn: 'root'
})
export class PatientService {
  
  private apiURL ='http://localhost:5210/api/v1/Patient';
  constructor(private http: HttpClient, private router: Router) { }

  getPatients() : Observable<Patient[]> {
     return this.http.get<Patient[]>(this.apiURL);
  }

  //get all with pagination
  getAll(page: number, pageSize: number): Observable<any> {
    return this.http.get<any>(`${this.apiURL}?page=${page}&pageSize=${pageSize}`);
  }

  //create
  createPatient(patient : Patient) : Observable<any> {
    return this.http.post<Patient>(this.apiURL, patient);
  }

  //update
  update(id: string, patient: Patient): Observable<Patient> {
    return this.http.put<Patient>(`${this.apiURL}/${id}`, patient);
  }

  //detail
  getById(id: string): Observable<Patient> {
    return this.http.get<Patient>(`${this.apiURL}/${id}`);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiURL}/${id}`);
  }

  checkEmailExists(email: string): Observable<boolean> {
    return this.http.get<{ exists: boolean }>(`${this.apiURL}/check-email?email=${email}`).pipe(
      map((response: any) => response.exists)
    );
  }
  logout(): void {
    // Clear user data from local storage or any other storage
    localStorage.removeItem('jwtToken');
    sessionStorage.removeItem('jwtToken');
    this.router.navigate(['']);    
  }
}
