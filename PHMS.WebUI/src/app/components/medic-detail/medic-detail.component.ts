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
type Section = 'profile' | 'editPersonalDetails' | 'editContactDetails' | 'editAddressDetails' | 'delete'  | 'security' | 'appointment';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';


@Component({
  selector: 'app-medic-detail',
  standalone: true,
  imports: [CommonModule, NavbarComponent, ReactiveFormsModule],
  templateUrl: './medic-detail.component.html',
  styleUrls: ['./medic-detail.component.css'],
  
})
export class MedicDetailComponent implements OnInit {
  medic?: Medic;
  appointments: Consultation[] = []; 
  patientDetails: Map<string, Patient> = new Map(); 
  activeSection: Section = 'profile'; // Secțiunea activă inițială
  editMode: boolean = false; // Mod de editare
  passwordRequired: boolean = false; // Pasul pentru introducerea parolei
  medicForm: FormGroup;
  passwordForm: FormGroup;
  errorMessage: string | null = null;


  constructor(
    private route: ActivatedRoute,
    private medicService: MedicService,
    private router: Router,
    private patientService: PatientService,
    private consultationService: ConsultationService, 
    private fb: FormBuilder


  ) {
    this.medicForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(30)]],
      lastName: ['', [Validators.required, Validators.maxLength(30)]],
      birthDate: ['', Validators.required],
      gender: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\+?[0-9]{7,15}$/)]],
      address: ['', Validators.required],
      rank: ['', Validators.required],
      specialization: ['', Validators.required],
      hospital: ['', Validators.required],
    });
    this.passwordForm = this.fb.group({
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.maxLength(100),
          Validators.pattern(/.*[A-Z].*/), 
          Validators.pattern(/.*[a-z].*/), 
          Validators.pattern(/.*[0-9].*/), 
          Validators.pattern(/.*[\W_].*/) 
        ]
      ]
    });
  }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      const token = sessionStorage.getItem('jwtToken'); // Retrieve the token from sessionStorage

      if (token) {
        this.medicService.getById(id).subscribe((data) => {
          this.medic = data;
          this.medicForm.patchValue(data); // Populate the form with patient data

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
setActiveSection(section: Section): void {
  this.activeSection = section;
  this.editMode = false; // Dezactivează modul de editare
  this.passwordRequired = false; // Dezactivează pasul pentru introducerea parolei
  this.errorMessage = null; // Resetează mesajul de eroare

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
  
  editPersonalDetails(): void {
    if (this.medic) {
      this.medicForm.patchValue(this.medic); // Populate the form with patient data
    }
    this.editMode = true;
    this.activeSection = 'editPersonalDetails';
  }
  editContactDetails(): void {
    if (this.medic) {
      this.medicForm.patchValue(this.medic); // Populate the form with patient data
    }
    this.editMode = true;
    this.activeSection = 'editContactDetails';
  }
  editAddressDetails(): void {
    if (this.medic) {
      this.medicForm.patchValue(this.medic); // Populate the form with patient data
    }
    this.editMode = true;
    this.activeSection = 'editAddressDetails';
  }

  cancelEdit(): void {
    this.editMode = false;
    this.passwordRequired = false;
    this.errorMessage = null; // Resetează mesajul de eroare
    this.activeSection = 'profile';
  }
  onSubmit(): void {
    if (this.medicForm.valid) {
      this.passwordRequired = true; // Trecem la pasul pentru introducerea parolei
    }
  }
  onUpdate(): void {
    if (this.passwordForm.valid) {
      const token = sessionStorage.getItem('jwtToken'); // Retrieve the token from sessionStorage
      
      if (token && this.medic) {
        const updatedMedic: Medic = { 
          ...this.medicForm.value, 
          id: this.medic.id,
          password: this.medic.passwordHash,
        };
  
        console.log('Updated Medic Data:', updatedMedic); // Verify that all fields are included
  
        // Add Authorization header
       
  
        // Send the request with the headers
        this.medicService.update(this.medic.id, updatedMedic, token).subscribe(
          () => {
            console.log('Medic updated successfully');
            this.editMode = false;
            this.passwordRequired = false;
            this.errorMessage = null;
            this.activeSection = 'profile';
            this.medic = updatedMedic; // Update the patient data
          },
          (error) => {
            console.error('Error updating medic:', error);
            this.errorMessage = this.extractErrorMessage(error); // Set the error message
          }
        );
      } else {
        console.error('No JWT token found in session storage');
      }
    }
  }
  
  
  private extractErrorMessage(error: any): string {
    if (error.error && typeof error.error === 'string') {
      return error.error;
    } else if (error.error && error.error.errors) {
      return Object.values(error.error.errors).flat().join(' ');
    } else {
      return 'Failed to update patient. Please try again.';
    }
  }

}