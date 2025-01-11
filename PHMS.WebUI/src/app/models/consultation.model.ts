import { Patient } from './patient.model';
import { Medic } from './medic.model';
export type Consultation = {
    patientId: string;
    medicId: string;
    date: string; 
    location: string;
    status: string;
  }
  