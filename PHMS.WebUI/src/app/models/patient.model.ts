import { User } from './user.model';

export interface Patient extends User {
  patientRecords: PatientRecord[];
}

export interface PatientRecord {
  patientRecordId: string;
  patientId: string;
  medicalCondition: string;
  treatment: string;
}