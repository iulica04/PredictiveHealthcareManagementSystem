import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from 'express';
import { Observable } from 'rxjs';
import { User } from '../models/user.model';
import { UserType } from '../models/userType.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl = 'http://localhost:5210/api/v1/User';
  constructor(private http: HttpClient, private router: Router) { }

  getUsersByType(type: UserType): Observable<User[]> {
    const url = `${this.apiUrl}/get?Type=${type}`;
    return this.http.get<User[]>(url);
  }

  //get all with pagination
  getAllFilteredPaginated(type: UserType, page: number, pageSize: number, rank?: string, specialization?: string, hospital?: string): Observable<any> {
    let url = `${this.apiUrl}/paginated?Type=${type}&Page=${page}&PageSize=${pageSize}`;
    if (rank) {
      url += `&rank=${rank}`;
    }
    if (specialization) {
      url += `&specialization=${specialization}`;
    }
    if (hospital) {
      url += `&hospital=${hospital}`;
    }
    return this.http.get<User[]>(url);
  }

  //update
  update(user: User, token: string): Observable<User> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.put<User>(`${this.apiUrl}/${user.id}`, user, { headers });
  }

  //detail
  getById(id: string, token: string): Observable<User> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.get<User>(`${this.apiUrl}/${id}`, { headers });
  }

  //delete
  delete(id: string, token: string): Observable<void> {
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers });
  }
}
