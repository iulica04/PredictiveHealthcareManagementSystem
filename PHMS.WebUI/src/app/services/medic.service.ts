import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Medic } from '../models/medic.model';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MedicService {

  private apiURL ='http://localhost:5210/api/v1/Medic';
  constructor(private http: HttpClient) { }

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
  update(id: string, medic: Medic): Observable<Medic> {
    return this.http.put<Medic>(`${this.apiURL}/${id}`, medic);
  }

  //detail
  getById(id: string): Observable<Medic> {
    return this.http.get<Medic>(`${this.apiURL}/${id}`);
  }

  //delete
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiURL}/${id}`);
  }
  checkEmailExists(email: string): Observable<boolean> {
    return this.http.get<{ exists: boolean }>(`${this.apiURL}/check-email?email=${email}`).pipe(
      map((response: any) => response.exists)
    );
  }
}
