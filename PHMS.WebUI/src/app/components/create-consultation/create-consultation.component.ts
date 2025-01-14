import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ConsultationService } from '../../services/consultation.service';
import { MedicService } from '../../services/medic.service';
import { Medic } from '../../models/medic.model';
import { Consultation } from '../../models/consultation.model';
import { PatientService } from '../../services/patient.service';
import { Patient } from '../../models/patient.model';
import { NavbarComponent } from '../navbar/navbar.component';
import { AbstractControl, ValidatorFn } from '@angular/forms'; // Add this line

@Component({
  selector: 'app-create-consultation',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NavbarComponent],
  templateUrl: './create-consultation.component.html',
  styleUrls: ['./create-consultation.component.css']
})
export class CreateConsultationComponent implements OnInit {
  consultationForm: FormGroup;
  currentStep: number = 1;
  nextClicked: boolean = false;
  currentImage: string = 'assets/images/image2.png';
  medics: Medic[] = [];  
  filteredMedics: Medic[] = []; // Array for filtered medics based on specialization
  specializations: string[] = [
    'Cardiology', 'Neurology', 'Orthopedics', 'Pediatrics', 'Dermatology', 'Oncology', 'Gastroenterology', 
    'Urology', 'Psychiatry', 'Internal Medicine', 'Endocrinology', 'Hematology', 'Infectious Diseases', 
    'Geriatrics', 'Immunology', 'Radiology', 'Rheumatology', 'Rehabilitation Medicine', 'Sports Medicine', 
    'Intensive Care Medicine', 'Toxicology', 'Genetics', 'Nutrition', 'Allergy', 'Family Medicine', 
    'Plastic Surgery', 'General Surgery', 'Cardiovascular Surgery', 'Thoracic Surgery', 'Pediatric Surgery', 
    'Neurosurgery', 'Obstetrics and Gynecology', 'Maxillofacial Surgery', 'Vascular Surgery', 'Bariatric Surgery', 
    'Endoscopic Surgery', 'Orthopedic Surgery', 'Oncological Surgery', 'Spinal Surgery', 'Plastic and Reconstructive Surgery'
  ];
  selectedSpecialization: string = ''; // Holds selected specialization
  patient?: Patient;

  timeRangeValidator(min: string, max: string): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      if (!control.value) return null; // Nu valida dacă nu este completat
      
      const [hour, minute] = control.value.split(':').map(Number);
      const inputTime = hour * 60 + minute;
      
      const [minHour, minMinute] = min.split(':').map(Number);
      const minTime = minHour * 60 + minMinute;
      
      const [maxHour, maxMinute] = max.split(':').map(Number);
      const maxTime = maxHour * 60 + maxMinute;
      
      return inputTime >= minTime && inputTime <= maxTime ? null : { outOfRange: true };
    };
  }

  constructor(
    private formBuilder: FormBuilder,
    private consultationService: ConsultationService,
    private medicService: MedicService,
    private patientService: PatientService,
    private router: Router
  ) {
    // Initialize form
    this.consultationForm = this.formBuilder.group({
      appointmentDate: ['', Validators.required],
      appointmentTime: ['', [Validators.required, this.timeRangeValidator('08:00', '18:00')]],
      medic: [null, Validators.required],
      specialization: ['']  // Add the specialization form control
    });
  }

  ngOnInit(): void {
    const patientId = sessionStorage.getItem('userId');

    if (patientId) {
      const token = sessionStorage.getItem('jwtToken');
      if (token) {
        this.patientService.getById(patientId, token).subscribe(
          (data: Patient) => {
            this.patient = data;
            this.populatePatientDetails();
          },
          (error) => {
            console.error('Error fetching patient details:', error);
          }
        );
      } else {
        console.error('No JWT token found in session storage');
      }
    }
    
    this.medicService.getMedics().subscribe(
      (data: Medic[]) => {
        this.medics = data;
        this.filteredMedics = data;  // Initialize with all medics
      },
      (error) => {
        console.error('Error fetching medics:', error);
        if (typeof window !== 'undefined') {
          alert('Failed to fetch medics. Please try again later.');
        }
      }
    );
  }

  populatePatientDetails(): void {
    if (this.patient) {
      this.consultationForm.addControl('firstName', this.formBuilder.control(this.patient.firstName));
      this.consultationForm.addControl('lastName', this.formBuilder.control(this.patient.lastName));
      this.consultationForm.addControl('email', this.formBuilder.control(this.patient.email));
      this.consultationForm.addControl('phoneNumber', this.formBuilder.control(this.patient.phoneNumber));
    }
  }

  // Method to filter medics based on selected specialization
  filterMedicsBySpecialization(): void {
    const specialization = this.consultationForm.get('specialization')?.value;
    this.selectedSpecialization = specialization;

    if (specialization) {
      this.filteredMedics = this.medics.filter(medic => medic.specialization === specialization);
    } else {
      this.filteredMedics = this.medics;
    }

    // Sort filtered medics by first and last name
    this.filteredMedics.sort((a, b) => {
      const nameA = a.firstName.toLowerCase() + ' ' + a.lastName.toLowerCase();
      const nameB = b.firstName.toLowerCase() + ' ' + b.lastName.toLowerCase();
      return nameA.localeCompare(nameB);
    });
  }

  previousStep(): void {
    this.currentStep--;
    this.updateImage();
  }

  validateStep1(): boolean {
    return (this.consultationForm.get('appointmentDate')?.valid ?? false) && 
           (this.consultationForm.get('appointmentTime')?.valid ?? false) &&
           (this.consultationForm.get('medic')?.valid ?? false);
  }

  validateStep2(): boolean {
    return this.consultationForm.valid;    
  }

  nextStep(): void {
    this.nextClicked = true;
    this.consultationForm.markAllAsTouched(); // Marchez toate câmpurile ca fiind atinse
  
    let isValid = false;
    switch (this.currentStep) {
      case 1:
        isValid = this.validateStep1();
        break;
      case 2:
        isValid = this.validateStep2();
        break;
    }
  
    if (isValid) {
      this.currentStep++;
      this.updateImage(); // Actualizează imaginea când treci la pasul următor
      this.nextClicked = false; // Resetează variabila când treci la pasul următor
    }
  }

  updateImage(): void {
    switch (this.currentStep) {
      case 1:
        this.currentImage = 'assets/images/image2.png';
        break;
      case 2:
        this.currentImage = 'assets/images/image3.png';
        break;
    }
  }

  onSubmit(): void {
    if (this.consultationForm.valid) {
      const patientId = sessionStorage.getItem('userId');
  
      if (!patientId) {
        console.error('No patient ID found in session');
        return;
      }
  
      const selectedMedic = this.consultationForm.get('medic')?.value;
      const date = this.consultationForm.get('appointmentDate')?.value;
      const time = this.consultationForm.get('appointmentTime')?.value;
  
      const localDateTime = new Date(`${date}T${time}`);
      const fullAppointmentDate = new Date(
        localDateTime.getTime() - localDateTime.getTimezoneOffset() * 60000
      ).toISOString();
  
      console.log('Local date and time:', localDateTime);
      console.log('UTC date to be sent:', fullAppointmentDate);
  
      const consultation: Consultation = {
        status: 0,
        patientId: patientId,
        medicId: selectedMedic.id,
        date: fullAppointmentDate,
        location: selectedMedic.hospital
      };
  
      console.log('Creating consultation:', consultation);
      this.consultationService.createConsultation(consultation).subscribe(
        (response) => {
          console.log('Consultation created successfully:', response);
          this.router.navigate(['']);
        },
        (error) => {
          console.error('Error creating consultation:', error);
        }
      );
    } else {
      console.log('Form is invalid');
    }
  }
}
