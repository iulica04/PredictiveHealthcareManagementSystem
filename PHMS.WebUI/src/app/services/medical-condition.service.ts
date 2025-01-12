import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MedicalCondition } from '../models/medicalCondition.model'; // Asigură-te că ai un model pentru MedicalCondition

@Injectable({
  providedIn: 'root'
})
export class MedicalConditionService {
  private apiURL = 'http://localhost:5210/api/v1/MedicalCondition'; // Înlocuiește cu endpoint-ul tău

  constructor(private http: HttpClient) { }

  getMedicalConditionsByPatientId(patientId: string): Observable<MedicalCondition[]> {
    return this.http.get<MedicalCondition[]>(`${this.apiURL}/patient/${patientId}`);
  }
}