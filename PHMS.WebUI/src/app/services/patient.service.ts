import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Patient } from '../models/patient.model';
import { Consultation } from '../models/consultation.model';

@Injectable({
  providedIn: 'root'
})
export class PatientService {
  
  private apiURL ='http://localhost:5210/api/v1/Patient';
  private apiUrl = 'http://localhost:5210/api/v1/Consultation';
  private medicUrl = 'http://localhost:5210/api/v1/Medic';
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
  update(id: string, patient: Patient, token: string): Observable<Patient> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  
    return this.http.put<Patient>(`${this.apiURL}/${id}`, patient, { headers });
  }

  updatePassword(passwordUpdate: {patientId: string, password: string }, token: string): Observable<void> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    console.log(passwordUpdate);
    return this.http.put<void>(`${this.apiURL}/${passwordUpdate.patientId}/update-password`, passwordUpdate, { headers });
  }

  //detail
  getById(id: string, token: string): Observable<Patient> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.get<Patient>(`${this.apiURL}/${id}`, { headers });
  }
  getByIdPatient(id: string): Observable<Patient> {
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
    sessionStorage.removeItem('jwtToken');
    sessionStorage.removeItem('userId');
    sessionStorage.removeItem('role');
    this.router.navigate(['']);    
  }
  getAllConsultations(token: string) {
    return this.http.get<Consultation[]>(`${this.apiUrl}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
  }
  getMedicById(medicId: string): Observable<any> {
    return this.http.get<any>(`${this.medicUrl}/${medicId}`);
  }
}
