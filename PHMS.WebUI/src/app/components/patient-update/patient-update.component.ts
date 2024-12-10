import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PatientService } from '../../services/patient.service';
import { Patient } from '../../models/patient.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-patient-update',
  standalone: true,
  imports: [ 
        CommonModule,
        ReactiveFormsModule
    ],
  templateUrl: './patient-update.component.html',
  styleUrl: './patient-update.component.css'
})
export class PatientUpdateComponent implements OnInit {
  patientForm: FormGroup;
  patientId!: string;

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private patientService: PatientService,
    private router: Router
  ) {
    this.patientForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(30)]],
      lastName: ['', [Validators.required, Validators.maxLength(30)]],
      birthDate: ['', Validators.required],
      gender: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\+?[0-9]{7,15}$/)]],
      address: ['', Validators.required],
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
    this.patientId = this.route.snapshot.paramMap.get('id') || '';
    if (this.patientId) {
      this.loadPatientData();
    }
  }

  loadPatientData(): void {
    const token = sessionStorage.getItem('jwtToken'); // Retrieve the token from sessionStorage

    if (token) {
      this.patientService.getById(this.patientId, token).subscribe(
        (patient: Patient) => {
          this.patientForm.patchValue(patient); // Populate the form with patient data
        },
        (error) => {
          console.error('Error loading patient data:', error);
          this.router.navigate(['/patients']); // Redirect in case of error
        }
      );
    } else {
      console.error('No JWT token found in session storage');
      this.router.navigate(['/patients']); // Redirect if no token is found
    }
  }


  onSubmit(): void {
    if (this.patientForm.valid) {
      const token = sessionStorage.getItem('jwtToken'); // Retrieve the token from sessionStorage
  
      if (token) {
        const UpdatesPatient: Patient = { ...this.patientForm.value, id: this.patientId };
  
        this.patientService.update(this.patientId, UpdatesPatient, token).subscribe(
          () => {
            console.log('Patient updated successfully');
            this.router.navigate(['/patients']);
          },
          (error) => {
            console.error('Error updating patient:', error);
          }
        );
      } else {
        console.error('No JWT token found in session storage');
      }
    }
  }
  
}
