import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PatientService } from '../../services/patient.service';
import { MedicalConditionService } from '../../services/medical-condition.service';
import { Patient } from '../../models/patient.model';
import { MedicalCondition } from '../../models/medicalCondition.model';
import { TreatmentType } from '../../models/treatment.model';
import { MedicationType } from '../../models/medication.model';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';

type Section = 'profile' | 'editPersonalDetails' | 'editContactDetails' | 'editAddressDetails' | 'delete' | 'treatments' | 'condition';

@Component({
  selector: 'app-patient-detail',
  templateUrl: './patient-detail.component.html',
  styleUrls: ['./patient-detail.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NavbarComponent] // Importă ReactiveFormsModule aici
})
export class PatientDetailComponent implements OnInit {
  patient: Patient | null = null;
  medicalConditions: MedicalCondition[] = [];
  activeSection: Section = 'profile'; // Secțiunea activă inițială
  editMode: boolean = false; // Mod de editare
  passwordRequired: boolean = false; // Pasul pentru introducerea parolei
  patientForm: FormGroup;
  passwordForm: FormGroup;
  errorMessage: string | null = null; // Variabilă pentru mesajul de eroare

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
        });
      }
    }
  }

  setActiveSection(section: Section): void {
    this.activeSection = section;
    this.editMode = false; // Dezactivează modul de editare
    this.passwordRequired = false; // Dezactivează pasul pentru introducerea parolei
    this.errorMessage = null; // Resetează mesajul de eroare
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
        const updatedPatient: Patient = { ...this.patientForm.value, id: this.patient.id, password: this.passwordForm.value.password };

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

  private extractErrorMessage(error: any): string {
    if (error.error && typeof error.error === 'string') {
      return error.error;
    } else if (error.error && error.error.errors) {
      return Object.values(error.error.errors).flat().join(' ');
    } else {
      return 'Failed to update patient. Please try again.';
    }
  }

  getTreatmentType(type: TreatmentType): string {
    switch (type) {
      case TreatmentType.Surgery:
        return 'Surgery';
      case TreatmentType.Therapy:
        return 'Therapy';
      case TreatmentType.Medication:
        return 'Medication';
      case TreatmentType.Rehabilitation:
        return 'Rehabilitation';
      case TreatmentType.Other:
        return 'Other';
      default:
        return 'Unknown';
    }
  }

  getMedicationType(type: MedicationType): string {
    switch (type) {
      case MedicationType.Tablet:
        return 'Tablet';
      case MedicationType.Capsule:
        return 'Capsule';
      case MedicationType.Liquid:
        return 'Liquid';
      case MedicationType.Injection:
        return 'Injection';
      case MedicationType.Inhaler:
        return 'Inhaler';
      case MedicationType.Topical:
        return 'Topical';
      case MedicationType.Suppository:
        return 'Suppository';
      case MedicationType.Drops:
        return 'Drops';
      case MedicationType.Other:
        return 'Other';
      default:
        return 'Unknown';
    }
  }

  formatDuration(duration: string): string {
    const date = new Date(duration);
    return `${date.getDate()}/${date.getMonth() + 1}/${date.getFullYear()} ${date.getHours()}:${date.getMinutes()}`;
  }
}