import { Medication } from './medication.model'; // Importă modelul Medication

export interface Treatment {
  treatmentId: string; // Guid în C# este string în TypeScript
  medicalConditionId: string; // Guid în C# este string în TypeScript
  type: TreatmentType;
  name: string;
  location: string;
  startDate: string; // DateTime în C# este string în TypeScript
  duration: string; // DateTime în C# este string în TypeScript
  frequency: string;
  medications: Medication[]; // Asigură-te că ai un model pentru Medication
}

export enum TreatmentType {
  Surgery,
  Therapy,
  Medication,
  Rehabilitation,
  Other
}