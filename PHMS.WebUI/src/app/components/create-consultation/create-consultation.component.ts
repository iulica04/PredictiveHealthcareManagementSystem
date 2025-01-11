import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ConsultationService } from '../../services/consultation.service';
import { MedicService } from '../../services/medic.service'; // Import the MedicService
import { Medic } from '../../models/medic.model'; // Import the Medic model
import { Consultation } from '../../models/consultation.model'; // Import the Consultation model

@Component({
  selector: 'app-create-consultation',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './create-consultation.component.html',
  styleUrls: ['./create-consultation.component.css']
})
export class CreateConsultationComponent implements OnInit {
  consultationForm: FormGroup;
  currentStep: number = 1;
  nextClicked: boolean = false;
  currentImage: string = 'assets/images/image2.png';
  medics: Medic[] = [];  // Store medic data

  constructor(
    private formBuilder: FormBuilder,
    private consultationService: ConsultationService,
    private medicService: MedicService,  // Inject MedicService
    private router: Router
  ) {
    this.consultationForm = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', Validators.required],
      appointmentDate: ['', Validators.required],
      appointmentType: ['', Validators.required],
      medicId: ['', Validators.required] // Form control for medic selection
    });
  }

  ngOnInit(): void {
    // Fetch available medics
    this.medicService.getMedics().subscribe(
      (data: Medic[]) => {
        this.medics = data; // Store the fetched medics
      },
      (error) => {
        console.error('Error fetching medics:', error);
        if (typeof window !== 'undefined') {
          alert('Failed to fetch medics. Please try again later.');
        }
      }
    );
  }

  // Step validation methods
  validateStep1(): boolean {
    return (this.consultationForm.get('firstName')?.valid ?? false) && 
           (this.consultationForm.get('lastName')?.valid ?? false) &&
           (this.consultationForm.get('email')?.valid ?? false) &&
           (this.consultationForm.get('phoneNumber')?.valid ?? false);
  }

  validateStep2(): boolean {
    return (this.consultationForm.get('appointmentDate')?.valid ?? false) &&
           (this.consultationForm.get('appointmentType')?.valid ?? false);
  }

  validateStep3(): boolean {
    return this.consultationForm.get('medicId')?.valid ?? false;
  }

  // Navigate to the next step
  nextStep(): void {
    this.nextClicked = true;
    this.consultationForm.markAllAsTouched();

    let isValid = false;
    switch (this.currentStep) {
      case 1:
        isValid = this.validateStep1();
        break;
      case 2:
        isValid = this.validateStep2();
        break;
      case 3:
        isValid = this.validateStep3();
        break;
    }

    if (isValid) {
      this.currentStep++;
      this.updateImage();
      this.nextClicked = false;
    }
  }

  // Navigate to the previous step
  previousStep(): void {
    this.currentStep--;
    this.updateImage();
  }

  // Update the image based on the current step
  updateImage(): void {
    switch (this.currentStep) {
      case 1:
        this.currentImage = 'assets/images/image2.png';
        break;
      case 2:
        this.currentImage = 'assets/images/image1.png';
        break;
      case 3:
        this.currentImage = 'assets/images/image3.png';
        break;
      case 4:
        this.currentImage = 'assets/images/image4.png'; // Image for Step 4
        break;
    }
  }

  // Submit the consultation form
  onSubmit(): void {
    if (this.consultationForm.valid) {
      const consultation: Consultation = this.consultationForm.value;

      // Call the service to create the consultation
      this.consultationService.createConsultation(consultation).subscribe(
        (response) => {
          console.log('Consultation created successfully:', response);
          this.router.navigate(['']);  // Redirect to another page after successful submission
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
