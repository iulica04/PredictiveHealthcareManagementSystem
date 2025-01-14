import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MedicalCondition } from '../models/medicalCondition.model'; 

@Injectable({
  providedIn: 'root'
})
export class MedicalConditionService {
  private apiURL = 'http://localhost:5210/api/v1/MedicalCondition'; 

  constructor(private http: HttpClient) { }

  getMedicalConditionsByPatientId(patientId: string): Observable<MedicalCondition[]> {
    return this.http.get<MedicalCondition[]>(`${this.apiURL}/patient/${patientId}`);
  }

  getMedicalConditionById(medicalConditionId: string): Observable<MedicalCondition> {
    return this.http.get<MedicalCondition>(`${this.apiURL}/${medicalConditionId}`);
  }

  createMedicalCondition(condition: MedicalCondition): Observable<any> {
    return this.http.post<MedicalCondition>(this.apiURL, condition);
  }

  updateMedicalCondition(medicalConditionId: string | null, condition: MedicalCondition): Observable<any> {
    return this.http.put<MedicalCondition>(`${this.apiURL}/${medicalConditionId}`, condition);
  }

}