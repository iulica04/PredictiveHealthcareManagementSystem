import { Treatment } from './treatment.model'; 

export interface MedicalCondition {
  medicalConditionId: string; 
  patientId: string; 
  name: string;
  description: string;
  startDate: string; 
  endDate?: string; 
  currentStatus: string;
  isGenetic?: boolean; 
  recommendation: string;
  treatments: Treatment[]; 
}