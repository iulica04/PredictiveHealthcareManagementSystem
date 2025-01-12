import { Component, OnInit } from '@angular/core';
import { Patient } from '../../models/patient.model';
import { Consultation } from '../../models/consultation.model';
import { ActivatedRoute, Router } from '@angular/router';
import { PatientService } from '../../services/patient.service';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { ConsultationStatus } from '../../models/consultation.model';

@Component({
  selector: 'app-patient-detail',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl: './patient-detail.component.html',
  styleUrls: ['./patient-detail.component.css'],
})
export class PatientDetailComponent implements OnInit {
  patient?: Patient;
  appointments: Consultation[] = [];  // Lista de programări
  medicDetails: Map<string, any> = new Map(); // Map pentru a salva detaliile medicilor

  constructor(
    private route: ActivatedRoute,
    private patientService: PatientService,
    private router: Router
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      const token = sessionStorage.getItem('jwtToken'); // Retrieve the token from sessionStorage

      if (token) {
        this.patientService.getById(id, token).subscribe((data) => {
          this.patient = data;
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
  navigateToUpdateConsultation(appointmentId: string): void {
    this.router.navigate(['/consultations/update', appointmentId]);
  }

  getAppointmentsId(appointment: any): string {
    return appointment.id; // Adjust this to match the actual ID property of your appointment object
  }
  

  getAppointments(patientId: string, token: string) {
    this.patientService.getAllConsultations(token).subscribe(
      (consultations) => {
        // Filtrare programări pentru pacientul curent
        this.appointments = consultations.filter(
          (consultation) => consultation.patientId === patientId
        );

        // Încarcă detaliile medicilor pentru fiecare programare
        this.loadMedicDetails();
      },
      (error) => {
        console.error('Failed to retrieve consultations:', error);
      }
    );
  }

  loadMedicDetails() {
    // Iterează prin programările pacientului pentru a obține detaliile medicilor
    for (const appointment of this.appointments) {
      if (!this.medicDetails.has(appointment.medicId)) {
        this.patientService.getMedicById(appointment.medicId).subscribe((medic) => {
          this.medicDetails.set(appointment.medicId, medic);
          console.log('Medic details:', medic);
        });
      }
    }
  }

  deletePatient() {
    if (this.patient) {
      this.patientService.delete(this.patient.id).subscribe(() => {
        this.router.navigate(['/patients']);
      });
    }
  }

  navigateToUpdatePatient(id: string) {
    this.router.navigate([`patients/update/${id}`]);
  }

  logout(): void {
    this.patientService.logout();
  }
}
