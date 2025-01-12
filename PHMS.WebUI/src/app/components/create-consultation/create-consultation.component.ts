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
      medic: [null, Validators.required] 
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
