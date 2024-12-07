import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { PatientService } from '../../services/patient.service';

@Component({
  selector: 'app-patient-create',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './patient-create.component.html',
  styleUrls: ['./patient-create.component.css']
})
export class PatientCreateComponent implements OnInit {
  patientForm: FormGroup;
  currentStep: number = 1; // Index pentru a urmări pasul curent
  nextClicked: boolean = false; // Variabilă pentru a urmări dacă butonul "Next" a fost apăsat
  currentImage: string = 'assets/images/image2.png'; // Variabilă pentru calea imaginii curente

  constructor(
    private fb: FormBuilder, 
    private patientService: PatientService,
    private router: Router) 
    {
    this.patientForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\+?[0-9]{7,15}$/)]],
      birthDate: ['', Validators.required],
      gender: ['', Validators.required],
      address: ['', Validators.required],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.maxLength(100),
          Validators.pattern(/.*[A-Z].*/), // Cel puțin o literă mare
          Validators.pattern(/.*[a-z].*/), // Cel puțin o literă mică
          Validators.pattern(/.*[0-9].*/), // Cel puțin o cifră
          Validators.pattern(/.*[\W_].*/)  // Cel puțin un caracter special
        ]
      ],
      confirmPassword: ['', Validators.required]
    }, { validator: this.passwordMatchValidator });
  }

  ngOnInit(): void {}

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
    } else {
      confirmPassword?.setErrors(null);
    }
  }

  validateStep1(): boolean {
    return (this.patientForm.get('firstName')?.valid ?? false) &&
           (this.patientForm.get('lastName')?.valid ?? false) &&
           (this.patientForm.get('email')?.valid ?? false) &&
           (this.patientForm.get('phoneNumber')?.valid ?? false);
  }

  validateStep2(): boolean {
    return (this.patientForm.get('birthDate')?.valid ?? false) &&
           (this.patientForm.get('gender')?.valid ?? false) &&
           (this.patientForm.get('address')?.valid ?? false);
  }

  validateStep3(): boolean {
    return (this.patientForm.get('password')?.valid ?? false) &&
           (this.patientForm.get('confirmPassword')?.valid ?? false);
  }

  nextStep(): void {
    this.nextClicked = true; 
    this.patientForm.markAllAsTouched(); // Marchez toate câmpurile ca fiind atinse

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
      this.updateImage(); // Actualizează imaginea când treci la pasul următor
      this.nextClicked = false; // Resetează variabila când treci la pasul următor
    }
  }

  previousStep(): void {
    this.currentStep--;
    this.updateImage(); // Actualizează imaginea când te întorci la pasul anterior
  }

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
    }
  }

  checkEmail(): void {
    const emailControl = this.patientForm.get('email');
    if (emailControl && emailControl.value) {
      this.patientService.checkEmailExists(emailControl.value).subscribe({
        next: (exists) => {
          if (exists) {
            emailControl.setErrors({ emailExists: true });
          } else {
            emailControl.setErrors(null);
          }
        },
        error: (error) => {
          console.error('Error checking email', error);
        }
      });
    }
  }


  onSubmit(): void {
    if (this.patientForm.valid) {
      const patient = { ...this.patientForm.value };
      delete patient.confirmPassword; // Elimină câmpul confirmPassword înainte de a trimite datele

      console.log('Patient created successfully', patient);

      this.patientService.createPatient(patient).subscribe({
        next: (response) => {
          console.log('Patient added to database', response);
          this.router.navigate(['/patients']);
        },
        error: (error) => {
          console.error('Error adding patient to database', error);
        }
      });
    }
  }

  // Funcții pentru a verifica fiecare criteriu de validare a parolei
  hasUpperCase(): boolean {
    const password = this.patientForm.get('password')?.value;
    return /[A-Z]/.test(password);
  }

  hasLowerCase(): boolean {
    const password = this.patientForm.get('password')?.value;
    return /[a-z]/.test(password);
  }

  hasNumber(): boolean {
    const password = this.patientForm.get('password')?.value;
    return /[0-9]/.test(password);
  }

  hasSpecialChar(): boolean {
    const password = this.patientForm.get('password')?.value;
    return /[\W_]/.test(password);
  }

  isValidLength(): boolean {
    const password = this.patientForm.get('password')?.value;
    return password && password.length >= 8;
  }

  getPasswordErrors(): string[] {
    const errors: string[] = [];
    if (!this.isValidLength()) {
      errors.push('at least 8 characters');
    }
    if (!this.hasUpperCase()) {
      errors.push('an uppercase letter');
    }
    if (!this.hasLowerCase()) {
      errors.push('a lowercase letter');
    }
    if (!this.hasNumber()) {
      errors.push('a digit');
    }
    if (!this.hasSpecialChar()) {
      errors.push('a special character');
    }
    return errors;
  }
}