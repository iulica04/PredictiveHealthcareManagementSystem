import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MedicService } from '../../services/medic.service';
import { Medic } from '../../models/medic.model';
import { Consultation } from '../../models/consultation.model';
import { ConsultationStatus } from '../../models/consultation.model';
import { ConsultationService } from '../../services/consultation.service';
import { Patient } from '../../models/patient.model';
import { PatientService } from '../../services/patient.service';
import { NavbarComponent } from '../navbar/navbar.component';


@Component({
  selector: 'app-medic-detail',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl: './medic-detail.component.html',
  styleUrls: ['./medic-detail.component.css']
})
export class MedicDetailComponent implements OnInit {
  medic?: Medic;
  appointments: Consultation[] = []; 
  patientDetails: Map<string, Patient> = new Map(); 


  constructor(
    private route: ActivatedRoute,
    private medicService: MedicService,
    private router: Router,
    private patientService: PatientService,
    private consultationService: ConsultationService, 


  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      const token = sessionStorage.getItem('jwtToken'); // Retrieve the token from sessionStorage

      if (token) {
        this.medicService.getById(id).subscribe((data) => {
          this.medic = data;
        });

        // Obține toate programările pacientului
        this.getAppointments(id, token);
      } else {
        console.error('No JWT token found in session storage');
      }
    }
  }
  getStatusString(status: number): string {
    return ConsultationStatus[status];
  }

getAppointments(medicId: string, token: string) {
  this.medicService.getAllConsultations(token).subscribe(
    
    (consultations) => {
      this.appointments = consultations.filter(
        (consultation) => consultation.medicId === medicId
      );
      this.loadPatientDetails(token);
    },
    
    (error) => {
      console.error('Failed to retrieve consultations:', error);
    }
  );
}
getAppointmentsId(appointment: any): string {
  return appointment.id; // Adjust this to match the actual ID property of your appointment object
}
navigateToUpdateConsultation(appointmentId: string): void {
  this.router.navigate(['/consultations/update', appointmentId]);
}

loadPatientDetails(token: string) {
  for (const appointment of this.appointments) {
    if (!this.patientDetails.has(appointment.patientId)) {
      this.patientService.getById(appointment.patientId, token).subscribe(
        (patient) => {
          this.patientDetails.set(appointment.patientId, patient);
          console.log('Patient details loaded:', patient);
        },
        (error) => {
          console.error('Error fetching patient details:', error);
        }
      );
    }
  }
}
  
  deleteMedic() {
    if (this.medic?.id) {
      const token = sessionStorage.getItem('jwtToken'); // Retrieve the token from sessionStorage

      if (token) {
        this.medicService.delete(this.medic.id, token).subscribe(
          () => {
            console.log('Medic deleted successfully');
            this.router.navigate(['medics']);
          },
          (error) => {
            console.error('Error deleting medic:', error);
          }
        );
      } else {
        console.error('No JWT token found in session storage');
      }
    }
  }
  navigateToUpdateMedic(id: string) {
    this.router.navigate([`medics/update/${id}`]);
  }
  logout(): void {
    this.medicService.logout();
  }
  deleteConsultation(appointmentId: string): void {
    const token = sessionStorage.getItem('jwtToken');
  
    if (token) {
      this.consultationService.deleteConsultation(appointmentId).subscribe(
        () => {
          console.log('Consultation deleted successfully');
          this.router.navigate(['']);

          // Actualizează lista de consultații
        },
        (error) => {
          console.error('Error deleting consultation:', error);
        }
      );
    } else {
      
      console.error('No JWT token found in session storage');
    }
  }
  

}