import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PatientService } from '../../services/patient.service';
import { MedicalConditionService } from '../../services/medical-condition.service';
import { Patient } from '../../models/patient.model';
import { MedicalCondition } from '../../models/medicalCondition.model';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { MedicalConditionGetComponent } from '../medical-condition-get/medical-condition-get.component';
import { Consultation } from '../../models/consultation.model';
import { ConsultationStatus } from '../../models/consultation.model';

type Section = 'profile' | 'editPersonalDetails' | 'editContactDetails' | 'editAddressDetails' | 'delete' | 'treatments' | 'condition' | 'security' | 'appointment';

@Component({
  selector: 'app-patient-detail',
  templateUrl: './patient-detail.component.html',
  styleUrls: ['./patient-detail.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NavbarComponent, MedicalConditionGetComponent] // Importă MedicalConditionGetComponent aici
})
export class PatientDetailComponent implements OnInit {
  patient: Patient | null = null;
  medicalConditions: MedicalCondition[] = [];
  activeSection: Section = 'profile'; // Secțiunea activă inițială
  editMode: boolean = false; // Mod de editare
  passwordRequired: boolean = false; // Pasul pentru introducerea parolei
  patientForm: FormGroup;
  passwordForm: FormGroup;
  resetPasswordForm: FormGroup;
  errorMessage: string | null = null; // Variabilă pentru mesajul de eroare
  appointments: Consultation[] = [];  // Lista de programări
  medicDetails: Map<string, any> = new Map(); // Map pentru a salva detaliile medicilor

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private patientService: PatientService,
    private medicalConditionService: MedicalConditionService,
    private fb: FormBuilder
  ) {
    this.patientForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(30)]],
      lastName: ['', [Validators.required, Validators.maxLength(30)]],
      birthDate: ['', Validators.required],
      gender: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\+?[0-9]{7,15}$/)]],
      address: ['', Validators.required]
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

    this.resetPasswordForm = this.fb.group({
      newPassword: [
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
      ],
      confirmPassword: [
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

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      const token = sessionStorage.getItem('jwtToken'); // Retrieve the token from sessionStorage

      if (token) {
        this.patientService.getById(id, token).subscribe((data) => {
          this.patient = data;
          this.patientForm.patchValue(data); // Populate the form with patient data
        });

        this.medicalConditionService.getMedicalConditionsByPatientId(id).subscribe((data: MedicalCondition[]) => {
          this.medicalConditions = data;
          this.getAppointments(id, token);

        });
      }
      else {
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

  setActiveSection(section: Section): void {
    this.activeSection = section;
    this.editMode = false; // Dezactivează modul de editare
    this.passwordRequired = false; // Dezactivează pasul pentru introducerea parolei
    this.errorMessage = null; // Resetează mesajul de eroare

    if (section === 'security') {
      this.resetPasswordForm.reset(); // Resetează formularul de schimbare a parolei
      this.passwordForm.reset(); // Resetează formularul de introducere a parolei curente
    }
  }

  deletePatient(): void {
    if (this.patient) {
      this.patientService.delete(this.patient.id).subscribe(() => {
        this.router.navigate(['/patients']);
      });
    }
  }

  editPersonalDetails(): void {
    if (this.patient) {
      this.patientForm.patchValue(this.patient); // Populate the form with patient data
    }
    this.editMode = true;
    this.activeSection = 'editPersonalDetails';
  }

  editContactDetails(): void {
    if (this.patient) {
      this.patientForm.patchValue(this.patient); // Populate the form with patient data
    }
    this.editMode = true;
    this.activeSection = 'editContactDetails';
  }

  editAddressDetails(): void {
    if (this.patient) {
      this.patientForm.patchValue(this.patient); // Populate the form with patient data
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
    if (this.patientForm.valid) {
      this.passwordRequired = true; // Trecem la pasul pentru introducerea parolei
    }
  }

  onUpdate(): void {
    if (this.passwordForm.valid) {
      const token = sessionStorage.getItem('jwtToken'); // Retrieve the token from sessionStorage

      if (token && this.patient) {
        const updatedPatient: Patient = { ...this.patientForm.value, id: this.patient.id, password: this.patient.passwordHash };

        console.log('Updated Patient Data:', updatedPatient); // Verifică datele trimise

        this.patientService.update(this.patient.id, updatedPatient, token).subscribe(
          () => {
            console.log('Patient updated successfully');
            this.editMode = false;
            this.passwordRequired = false;
            this.errorMessage = null; // Resetează mesajul de eroare
            this.activeSection = 'profile';
            this.patient = updatedPatient; // Update the patient data
          },
          (error) => {
            console.error('Error updating patient:', error);
            this.errorMessage = this.extractErrorMessage(error); // Setează mesajul de eroare specific
          }
        );
      } else {
        console.error('No JWT token found in session storage');
      }
    }
  }

  onResetPassword(): void {
    if (this.resetPasswordForm.valid) {
      const newPassword = this.resetPasswordForm.value.newPassword;
      const confirmPassword = this.resetPasswordForm.value.confirmPassword;

      if (newPassword !== confirmPassword) {
        this.errorMessage = 'Passwords do not match';
        return;
      }

      this.passwordRequired = true; // Trecem la pasul pentru introducerea parolei curente
    }
  }

  onUpdatePassword(): void {
    if (this.passwordForm.valid) {
      const password = this.resetPasswordForm.value.newPassword;
  
      if (this.patient) {
        const passwordUpdate = { patientId: this.patient.id, password };
  
        const token = sessionStorage.getItem('jwtToken'); // Retrieve the token from sessionStorage
  
        if (token) {
          this.patientService.updatePassword(passwordUpdate, token).subscribe(
            () => {
              console.log('Password updated successfully');
              this.errorMessage = null; // Resetează mesajul de eroare
              this.passwordRequired = false;
              this.activeSection = 'profile';
            },
            (error) => {
              console.error('Error updating password:', error);
              this.errorMessage = this.extractErrorMessage(error); // Setează mesajul de eroare specific
            }
          );
        } else {
          console.error('No JWT token found in session storage');
        }
      }
    }
  }

  toggleEditMode(): void {
    this.editMode = !this.editMode;
  }

  togglePasswordRequired(): void {
    this.passwordRequired = !this.passwordRequired;
  }


  viewMedicDetails(medicId: string): void {
    this.router.navigate(['/medics', medicId]);
  }

  getPasswordErrorMessage(controlName: string): string {
    const control = this.resetPasswordForm.get(controlName);
    if (control?.hasError('required')) {
      return 'Password is required';
    } else if (control?.hasError('minlength')) {
      return 'Password must be at least 8 characters long';
    } else if (control?.hasError('maxlength')) {
      return 'Password cannot be more than 100 characters long';
    } else if (control?.hasError('pattern')) {
      const patternError = control.errors?.['pattern'];
      if (patternError.requiredPattern.includes('[A-Z]')) {
        return 'Password must contain at least one uppercase letter';
      } else if (patternError.requiredPattern.includes('[a-z]')) {
        return 'Password must contain at least one lowercase letter';
      } else if (patternError.requiredPattern.includes('[0-9]')) {
        return 'Password must contain at least one number';
      } else if (patternError.requiredPattern.includes('[\\W_]')) {
        return 'Password must contain at least one special character';
      }
    }
    return '';
  }
  
  doPasswordsMatch(): boolean {
    const newPassword = this.resetPasswordForm.get('newPassword')?.value;
    const confirmPassword = this.resetPasswordForm.get('confirmPassword')?.value;
    return newPassword === confirmPassword;
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
  navigateToUpdatePatient(id: string) {
    this.router.navigate([`patients/update/${id}`]);
  }
}
