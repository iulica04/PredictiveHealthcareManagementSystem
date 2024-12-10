import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Medic } from '../models/medic.model';
import { map, Observable } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class MedicService {

  private apiURL ='http://localhost:5210/api/v1/Medic';
  constructor(private http: HttpClient, private router: Router) { }

  getMedics() : Observable<Medic[]> {
    return this.http.get<Medic[]>(this.apiURL);
  }

  //get all with pagination
  getAll(page: number, pageSize: number, rank?: string, specialization?: string): Observable<any> {
    let url = `${this.apiURL}/paginated?page=${page}&pageSize=${pageSize}`;
    if (rank) {
        url += `&rank=${rank}`;
    }
    if (specialization) {
        url += `&specialization=${specialization}`;
    }
    return this.http.get<Medic[]>(url);
  }

  //create
  createMedic(medic : Medic) : Observable<any> {
    return this.http.post<Medic>(this.apiURL, medic);
  }

  //update
  update(id: string, medic: Medic, token: string): Observable<Medic> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  
    return this.http.put<Medic>(`${this.apiURL}/${id}`, medic, { headers });
  }
  

  //detail
  getById(id: string): Observable<Medic> {
    return this.http.get<Medic>(`${this.apiURL}/${id}`);
  }

  //delete
  delete(id: string, token: string): Observable<void> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  
    return this.http.delete<void>(`${this.apiURL}/${id}`, { headers });
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
}
