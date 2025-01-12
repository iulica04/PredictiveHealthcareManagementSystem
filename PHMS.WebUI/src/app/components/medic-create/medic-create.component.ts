import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MedicService } from '../../services/medic.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-medic-create',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './medic-create.component.html',
  styleUrls: ['./medic-create.component.css']  // Make sure this path matches your actual file
})
export class MedicCreateComponent implements OnInit {
  medicForm: FormGroup;
  currentStep: number = 1;
  nextClicked: boolean = false;
  currentImage: string = 'assets/images/image2.png';

  constructor(
    private fb: FormBuilder,
    private medicService: MedicService,
    private router: Router
  ) {
    // Initialize the form group with appropriate form controls and validation
    this.medicForm = this.fb.group({
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
          Validators.pattern(/.*[A-Z].*/), // Must have at least one uppercase letter
          Validators.pattern(/.*[a-z].*/), // Must have at least one lowercase letter
          Validators.pattern(/.*[0-9].*/), // Must have at least one number
          Validators.pattern(/.*[\W_].*/)  // Must have at least one special character
        ]
      ],
      confirmPassword: ['', Validators.required],
      rank: ['', [Validators.required, Validators.maxLength(30)]],
      specialization: ['', [Validators.required, Validators.maxLength(30)]],
      hospital: ['', [Validators.required, Validators.maxLength(30)]]
    }, { validator: this.passwordMatchValidator });
  }

  ngOnInit(): void {}

  // Custom validator to check if password and confirmPassword match
  passwordMatchValidator(formGroup: FormGroup): void {
    const password = formGroup.get('password');
    const confirmPassword = formGroup.get('confirmPassword');
    if (password && confirmPassword) {
      confirmPassword.setErrors(
        password.value === confirmPassword.value ? null : { mismatch: true }
      );
    }
  }
  
  validateStep1(): boolean {
    return (this.medicForm.get('firstName')?.valid ?? false) &&
           (this.medicForm.get('lastName')?.valid ?? false) &&
           (this.medicForm.get('email')?.valid ?? false) &&
           (this.medicForm.get('phoneNumber')?.valid ?? false);
  }

  validateStep2(): boolean {
    return (this.medicForm.get('birthDate')?.valid ?? false) &&
           (this.medicForm.get('gender')?.valid ?? false) &&
           (this.medicForm.get('address')?.valid ?? false);
  }

  validateStep3(): boolean {
    return (this.medicForm.get('password')?.valid ?? false) &&
           (this.medicForm.get('confirmPassword')?.valid ?? false);
  }

  validateStep4(): boolean {
    return (this.medicForm.get('specialization')?.valid ?? false) &&
           (this.medicForm.get('rank')?.valid ?? false) &&
           (this.medicForm.get('hospital')?.valid ?? false); // Fixed the missing condition.
  }
  

  nextStep(): void {
    this.nextClicked = true; 
    this.medicForm.markAllAsTouched(); // Marchez toate câmpurile ca fiind atinse

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
      case 4:
        isValid = this.validateStep4();
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
      case 4:
        this.currentImage = 'assets/images/image4.png'; // Example for Step 4
        break;
    }
  }
  

  checkEmail(): void {
    const emailControl = this.medicForm.get('email');
    if (emailControl?.value) {
      this.medicService.checkEmailExists(emailControl.value).subscribe({
        next: (exists) => {
          if (exists) {
            emailControl.setErrors({ emailExists: true });
          } else if (emailControl.hasError('emailExists')) {
            emailControl.setErrors(null); // Only remove emailExists error
          }
        },
        error: (error) => {
          console.error('Error checking email', error);
        }
      });
    }
  }
  


  // Submit method to send the form data to the service
 
  onSubmit(): void {
    if (this.medicForm.valid) {
      const medic = { ...this.medicForm.value };
      delete medic.confirmPassword; // Elimină câmpul confirmPassword înainte de a trimite datele

      console.log('Medic created successfully', medic);

      this.medicService.createMedic(medic).subscribe({
        next: (response) => {
          console.log('Medic added to database', response);
          this.router.navigate(['/medics']);
        },
        error: (error) => {
          console.error('Error adding medic to database', error);
        }
      });
    }
  }
  hasUpperCase(): boolean {
    const password = this.medicForm.get('password')?.value;
    return /[A-Z]/.test(password);
  }

  hasLowerCase(): boolean {
    const password = this.medicForm.get('password')?.value;
    return /[a-z]/.test(password);
  }

  hasNumber(): boolean {
    const password = this.medicForm.get('password')?.value;
    return /[0-9]/.test(password);
  }

  hasSpecialChar(): boolean {
    const password = this.medicForm.get('password')?.value;
    return /[\W_]/.test(password);
  }

  isValidLength(): boolean {
    const password = this.medicForm.get('password')?.value;
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
