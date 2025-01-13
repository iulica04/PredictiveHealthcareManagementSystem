export enum ConsultationStatus {
  Pending = 0,
  Accepted = 1,
  Cancelled = 2,
  Done = 3,
  Declined = 4
}
export type Consultation = {
    patientId: string;
    medicId: string;
    date: string; 
    location: string;
    status: ConsultationStatus;
  }
  