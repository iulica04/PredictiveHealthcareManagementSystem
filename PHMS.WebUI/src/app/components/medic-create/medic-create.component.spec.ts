import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { MedicCreateComponent } from './medic-create.component';
import { MedicService } from '../../services/medic.service';
import { By } from '@angular/platform-browser';

fdescribe('MedicCreateComponent', () => {
  let component: MedicCreateComponent;
  let fixture: ComponentFixture<MedicCreateComponent>;
  let medicServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    medicServiceMock = jasmine.createSpyObj('MedicService', ['checkEmailExists', 'createMedic']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, MedicCreateComponent],
      providers: [
        { provide: MedicService, useValue: medicServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MedicCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should validate step 1', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '',
      gender: '',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '',
      password: '',
      confirmPassword: '',
      rank: '',
      specialization: '',
      hospital: ''
    });
    expect(component.validateStep1()).toBeTrue();
  });

  it('should validate step 2', () => {
    component.medicForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '1990-01-01',
      gender: 'Male',
      email: '',
      phoneNumber: '',
      address: '123 Main St',
      password: '',
      confirmPassword: '',
      rank: '',
      specialization: '',
      hospital: ''
    });
    expect(component.validateStep2()).toBeTrue();
  });

  it('should validate step 3', () => {
    component.medicForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      password: 'Password123!',
      confirmPassword: 'Password123!',
      rank: '',
      specialization: '',
      hospital: ''
    });
    expect(component.validateStep3()).toBeTrue();
  });

  it('should validate step 4', () => {
    component.medicForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      password: '',
      confirmPassword: '',
      rank: 'Doctor',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
    });
    expect(component.validateStep4()).toBeTrue();
  });

  it('should navigate to the next step if the current step is valid', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '',
      gender: '',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '',
      password: '',
      confirmPassword: '',
      rank: '',
      specialization: '',
      hospital: ''
    });
    component.nextStep();
    expect(component.currentStep).toBe(2);
  });

  it('should not navigate to the next step if the current step is invalid', () => {
    component.medicForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      password: '',
      confirmPassword: '',
      rank: '',
      specialization: '',
      hospital: ''
    });
    component.nextStep();
    expect(component.currentStep).toBe(1);
  });

  it('should navigate to the previous step', () => {
    component.currentStep = 2;
    component.previousStep();
    expect(component.currentStep).toBe(1);
  });

  it('should update image based on current step', () => {
    component.currentStep = 1;
    component.updateImage();
    expect(component.currentImage).toBe('assets/images/image2.png');

    component.currentStep = 2;
    component.updateImage();
    expect(component.currentImage).toBe('assets/images/image1.png');

    component.currentStep = 3;
    component.updateImage();
    expect(component.currentImage).toBe('assets/images/image3.png');

    component.currentStep = 4;
    component.updateImage();
    expect(component.currentImage).toBe('assets/images/image4.png');
  });

  it('should check if email exists', () => {
    medicServiceMock.checkEmailExists.and.returnValue(of(false));
    component.medicForm.get('email')?.setValue('john.doe@example.com');
    component.checkEmail();
    expect(medicServiceMock.checkEmailExists).toHaveBeenCalledWith('john.doe@example.com');
  });

  it('should show error if email exists', () => {
    medicServiceMock.checkEmailExists.and.returnValue(of(true));
    component.medicForm.get('email')?.setValue('john.doe@example.com');
    component.checkEmail();
    expect(component.medicForm.get('email')?.hasError('emailExists')).toBeTrue();
  });

  it('should submit the form if valid', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1990-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      password: 'Password123!',
      confirmPassword: 'Password123!',
      rank: 'Doctor',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
    });
    medicServiceMock.createMedic.and.returnValue(of({}));
    component.onSubmit();
    expect(medicServiceMock.createMedic).toHaveBeenCalled();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should not submit the form if invalid', () => {
    component.medicForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      password: '',
      confirmPassword: '',
      rank: '',
      specialization: '',
      hospital: ''
    });
    component.onSubmit();
    expect(medicServiceMock.createMedic).not.toHaveBeenCalled();
  });

  it('should show error message on form submission failure', () => {
    spyOn(console, 'error');
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1990-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      password: 'Password123!',
      confirmPassword: 'Password123!',
      rank: 'Doctor',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
    });
    medicServiceMock.createMedic.and.returnValue(throwError({ error: 'Error adding medic to database' }));
    component.onSubmit();
    expect(console.error).toHaveBeenCalledWith('Error adding medic to database', { error: 'Error adding medic to database' });
  });

  it('should validate password requirements', () => {
    component.medicForm.get('password')?.setValue('Password123!');
    expect(component.hasUpperCase()).toBeTrue();
    expect(component.hasLowerCase()).toBeTrue();
    expect(component.hasNumber()).toBeTrue();
    expect(component.hasSpecialChar()).toBeTrue();
    expect(component.isValidLength()).toBeTrue();
  });

  it('should return password errors', () => {
    component.medicForm.get('password')?.setValue('pass');
    const errors = component.getPasswordErrors();
    expect(errors).toContain('at least 8 characters');
    expect(errors).toContain('an uppercase letter');
    expect(errors).toContain('a digit');
    expect(errors).toContain('a special character');
  });

  it('should set password mismatch error', () => {
    component.medicForm.get('password')?.setValue('Password123!');
    component.medicForm.get('confirmPassword')?.setValue('Password123');
    component.passwordMatchValidator(component.medicForm);
    expect(component.medicForm.get('confirmPassword')?.hasError('mismatch')).toBeTrue();
  });

  it('should not set password mismatch error when passwords match', () => {
    component.medicForm.get('password')?.setValue('Password123!');
    component.medicForm.get('confirmPassword')?.setValue('Password123!');
    component.passwordMatchValidator(component.medicForm);
    expect(component.medicForm.get('confirmPassword')?.hasError('mismatch')).toBeFalse();
  });

  it('should display first name required error when first name is touched and empty', () => {
    const firstNameControl = component.medicForm.get('firstName');
    firstNameControl?.markAsTouched();
    firstNameControl?.setValue('');
    fixture.detectChanges();
    const firstNameError = fixture.debugElement.query(By.css('.error')).nativeElement;
    expect(firstNameError.textContent).toContain('First Name is required');
  });

  it('should display last name required error when last name is touched and empty', () => {
    const lastNameControl = component.medicForm.get('lastName');
    lastNameControl?.markAsTouched();
    lastNameControl?.setValue('');
    fixture.detectChanges();
    const lastNameError = fixture.debugElement.query(By.css('.error')).nativeElement;
    expect(lastNameError.textContent).toContain('Last Name is required');
  });

  it('should display email required error when email is touched and empty', () => {
    const emailControl = component.medicForm.get('email');
    emailControl?.markAsTouched();
    emailControl?.setValue('');
    fixture.detectChanges();
    const emailError = fixture.debugElement.query(By.css('.error')).nativeElement;
    expect(emailError.textContent).toContain('Email is required');
  });

  it('should display invalid email format error when email is touched and invalid', () => {
    const emailControl = component.medicForm.get('email');
    emailControl?.markAsTouched();
    emailControl?.setValue('invalid-email');
    fixture.detectChanges();
    const emailError = fixture.debugElement.query(By.css('.error')).nativeElement;
    expect(emailError.textContent).toContain('Invalid email format');
  });

  it('should display phone number required error when phone number is touched and empty', () => {
    const phoneNumberControl = component.medicForm.get('phoneNumber');
    phoneNumberControl?.markAsTouched();
    phoneNumberControl?.setValue('');
    fixture.detectChanges();
    const phoneNumberError = fixture.debugElement.query(By.css('.error')).nativeElement;
    expect(phoneNumberError.textContent).toContain('Phone Number is required');
  });

  it('should display invalid phone number format error when phone number is touched and invalid', () => {
    const phoneNumberControl = component.medicForm.get('phoneNumber');
    phoneNumberControl?.markAsTouched();
    phoneNumberControl?.setValue('invalid-phone');
    fixture.detectChanges();
    const phoneNumberError = fixture.debugElement.query(By.css('.error')).nativeElement;
    expect(phoneNumberError.textContent).toContain('Invalid phone number format');
  });

  it('should display birth date required error when birth date is touched and empty', () => {
    component.currentStep = 2;
    const birthDateControl = component.medicForm.get('birthDate');
    birthDateControl?.markAsTouched();
    birthDateControl?.setValue('');
    fixture.detectChanges();
    const birthDateError = fixture.debugElement.query(By.css('.error')).nativeElement;
    expect(birthDateError.textContent).toContain('Birth Date is required');
  });

  it('should display gender required error when gender is touched and empty', () => {
    component.currentStep = 2;
    const genderControl = component.medicForm.get('gender');
    genderControl?.markAsTouched();
    genderControl?.setValue('');
    fixture.detectChanges();
    const genderError = fixture.debugElement.query(By.css('.error')).nativeElement;
    expect(genderError.textContent).toContain('Gender is required');
  });

  it('should display address required error when address is touched and empty', () => {
    component.currentStep = 2;
    const addressControl = component.medicForm.get('address');
    addressControl?.markAsTouched();
    addressControl?.setValue('');
    fixture.detectChanges();
    const addressError = fixture.debugElement.query(By.css('.error')).nativeElement;
    expect(addressError.textContent).toContain('Address is required');
  });

  it('should display password required error when password is touched and empty', () => {
    component.currentStep = 3;
    const passwordControl = component.medicForm.get('password');
    passwordControl?.markAsTouched();
    passwordControl?.setValue('');
    fixture.detectChanges();
    const passwordError = fixture.debugElement.query(By.css('.error')).nativeElement;
    expect(passwordError.textContent).toContain('Password is required');
  });


  it('should display rank required error when rank is touched and empty', () => {
    component.currentStep = 4;
    const rankControl = component.medicForm.get('rank');
    rankControl?.markAsTouched();
    rankControl?.setValue('');
    fixture.detectChanges();
    const rankError = fixture.debugElement.query(By.css('.error')).nativeElement;
    expect(rankError.textContent).toContain('Rank is required');
  });

  it('should display specialization required error when specialization is touched and empty', () => {
    component.currentStep = 4;
    const specializationControl = component.medicForm.get('specialization');
    specializationControl?.markAsTouched();
    specializationControl?.setValue('');
    fixture.detectChanges();
    const specializationError = fixture.debugElement.query(By.css('.error')).nativeElement;
    expect(specializationError.textContent).toContain('Specialization is required');
  });

  it('should display hospital required error when hospital is touched and empty', () => {
    component.currentStep = 4;
    const hospitalControl = component.medicForm.get('hospital');
    hospitalControl?.markAsTouched();
    hospitalControl?.setValue('');
    fixture.detectChanges();
    const hospitalError = fixture.debugElement.query(By.css('.error')).nativeElement;
    expect(hospitalError.textContent).toContain('Hospital is required');
  });

  it('should not set mismatch error when passwords are null', () => {
    component.medicForm.get('password')?.setValue(null);
    component.medicForm.get('confirmPassword')?.setValue(null);
    component.passwordMatchValidator(component.medicForm);
    expect(component.medicForm.get('confirmPassword')?.hasError('mismatch')).toBeFalse();
  });

  it('should mark form as valid when all fields are correctly filled', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1990-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      password: 'Password123!',
      confirmPassword: 'Password123!',
      rank: 'Doctor',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
    });
    expect(component.medicForm.valid).toBeTrue();
  });
  
  
});