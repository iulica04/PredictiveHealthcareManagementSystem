import { PatientRecord } from "./patientRecord.model";
import { UserType } from "./userType.model";

export interface User {
    id: string;
    type: UserType;
    firstName: string;
    lastName: string;
    birthDate: string;          
    gender: string;
    email: string;
    passwordHash: string;       
    phoneNumber: string;
    address: string;

    // Medic specific
    rank?: string;
    specialization?: string;
    hospital?: string;

    // Patient specific
    patientRecords?: PatientRecord[];
  }