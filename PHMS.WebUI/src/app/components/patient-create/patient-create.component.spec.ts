import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PatientCreateComponent } from './patient-create.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { EmailVerificationService } from '../../services/email-verification.service';
import { PatientService } from '../../services/patient.service';
import { Router } from '@angular/router'; // Importă Router corect

describe('PatientCreateComponent', () => {
  let component: PatientCreateComponent;
  let fixture: ComponentFixture<PatientCreateComponent>;
  let patientServiceMock: any;
  let routerMock: any;
  let emailVerificationServiceMock: any;

  beforeEach(async () => {
    patientServiceMock = jasmine.createSpyObj('PatientService', ['createPatient']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);
    emailVerificationServiceMock = jasmine.createSpyObj('EmailVerificationService', ['checkEmailExists']);

    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        HttpClientTestingModule,
        ReactiveFormsModule,
        PatientCreateComponent // Asigură-te că componenta este inclusă în imports
      ],
      providers: [
        { provide: PatientService, useValue: patientServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: EmailVerificationService, useValue: emailVerificationServiceMock } // Adaugă EmailVerificationService
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PatientCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should not call PatientService.createPatient if form is invalid', () => {
    component.patientForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      password: '',
      confirmPassword: '' // Asigură-te că acest câmp este setat
    });

    component.onSubmit();

    expect(patientServiceMock.createPatient).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });
});
