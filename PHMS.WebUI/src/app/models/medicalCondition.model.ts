import { Treatment } from './treatment.model'; // Importă modelul Treatment

export interface MedicalCondition {
  medicalConditionId: string; // Guid în C# este string în TypeScript
  patientId: string; // Guid în C# este string în TypeScript
  name: string;
  description: string;
  startDate: string; // DateTime în C# este string în TypeScript
  endDate?: string; // DateTime? în C# este string? în TypeScript
  currentStatus: string;
  isGenetic?: boolean; // Boolean? în C# este boolean? în TypeScript
  recommendation: string;
  treatments: Treatment[]; // Asigură-te că ai un model pentru Treatment
}