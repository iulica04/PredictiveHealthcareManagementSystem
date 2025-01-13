import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MedicalCondition } from '../models/medicalcondition.model';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class MedicalConditionService {
  private apiURL = 'http://localhost:5210/api/v1/MedicalCondition';

  constructor(private http: HttpClient, private router: Router) {}

  // POST: Crează o condiție medicală
  createMedicalCondition(condition: MedicalCondition): Observable<any> {
    return this.http.post<MedicalCondition>(this.apiURL, condition);
  }
 
}