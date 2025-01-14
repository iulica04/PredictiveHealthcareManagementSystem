import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { PatientDetailComponent } from './patient-detail.component';
import { PatientService } from '../../services/patient.service';
import { MedicalConditionService } from '../../services/medical-condition.service';
import { Patient } from '../../models/patient.model';
import { MedicalCondition } from '../../models/medicalCondition.model';
import { CommonModule } from '@angular/common';

fdescribe('PatientDetailComponent', () => {
  let component: PatientDetailComponent;
  let fixture: ComponentFixture<PatientDetailComponent>;
  let patientServiceMock: any;
  let medicalConditionServiceMock: any;
  let routerMock: any;
  let activatedRouteMock: any;

  beforeEach(async () => {
    patientServiceMock = jasmine.createSpyObj('PatientService', ['getById', 'delete', 'update', 'getAllConsultations', 'getMedicById', 'updatePassword']);
    medicalConditionServiceMock = jasmine.createSpyObj('MedicalConditionService', ['getMedicalConditionsByPatientId']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);
    activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy().and.returnValue('1')
        }
      }
    };

    await TestBed.configureTestingModule({
      imports: [CommonModule, ReactiveFormsModule, PatientDetailComponent],
      providers: [
        { provide: PatientService, useValue: patientServiceMock },
        { provide: MedicalConditionService, useValue: medicalConditionServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PatientDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load patient details on init', () => {
    const patient: Patient = { id: '1', firstName: 'Jane', lastName: 'Doe', email: 'jane.doe@example.com', phoneNumber: '+1234567890', birthDate: '1990-01-01', gender: 'Female', address: '123 Main St', passwordHash: 'hashedPassword', patientRecords: [] };
    patientServiceMock.getById.and.returnValue(of(patient));
    medicalConditionServiceMock.getMedicalConditionsByPatientId.and.returnValue(of([]));
    spyOn(sessionStorage, 'getItem').and.callFake((key: string) => {
      if (key === 'jwtToken') return 'token';
      return null;
    });

    component.ngOnInit();
    expect(patientServiceMock.getById).toHaveBeenCalledWith('1', 'token');
    expect(component.patient).toEqual(patient);
  });

  it('should handle error when loading patient details', () => {
    spyOn(console, 'error');
    patientServiceMock.getById.and.returnValue(throwError({ error: 'Error loading patient details' }));
    spyOn(sessionStorage, 'getItem').and.callFake((key: string) => {
      if (key === 'jwtToken') return 'token';
      return null;
    });

    component.ngOnInit();
    expect(console.error).toHaveBeenCalledWith('Error fetching patient details:', { error: 'Error loading patient details' });
  });

  it('should delete patient', () => {
    const patient: Patient = { id: '1', firstName: 'Jane', lastName: 'Doe', email: 'jane.doe@example.com', phoneNumber: '+1234567890', birthDate: '1990-01-01', gender: 'Female', address: '123 Main St', passwordHash: 'hashedPassword', patientRecords: [] };
    component.patient = patient;
    patientServiceMock.delete.and.returnValue(of({}));

    component.deletePatient();

    expect(patientServiceMock.delete).toHaveBeenCalledWith('1');
    expect(routerMock.navigate).toHaveBeenCalledWith(['/patients']);
  });

  it('should handle error when deleting patient', () => {
    spyOn(console, 'error');
    const patient: Patient = { id: '1', firstName: 'Jane', lastName: 'Doe', email: 'jane.doe@example.com', phoneNumber: '+1234567890', birthDate: '1990-01-01', gender: 'Female', address: '123 Main St', passwordHash: 'hashedPassword', patientRecords: [] };
    component.patient = patient;
    patientServiceMock.delete.and.returnValue(throwError({ error: 'Error deleting patient' }));

    component.deletePatient();

    expect(console.error).toHaveBeenCalledWith('Error deleting patient:', { error: 'Error deleting patient' });
  });

  it('should update patient details', () => {
    const patient: Patient = { id: '1', firstName: 'Jane', lastName: 'Doe', email: 'jane.doe@example.com', phoneNumber: '+1234567890', birthDate: '1990-01-01', gender: 'Female', address: '123 Main St', passwordHash: 'hashedPassword', patientRecords: [] };
    component.patient = patient;
    component.patientForm.patchValue(patient);
    patientServiceMock.update.and.returnValue(of({}));
    spyOn(sessionStorage, 'getItem').and.callFake((key: string) => {
      if (key === 'jwtToken') return 'token';
      return null;
    });

    component.onUpdate();

    expect(patientServiceMock.update).toHaveBeenCalledWith('1', jasmine.any(Object), 'token');
    expect(component.editMode).toBeFalse();
    expect(component.passwordRequired).toBeFalse();
  });

  it('should handle error when updating patient details', () => {
    spyOn(console, 'error');
    const patient: Patient = { id: '1', firstName: 'Jane', lastName: 'Doe', email: 'jane.doe@example.com', phoneNumber: '+1234567890', birthDate: '1990-01-01', gender: 'Female', address: '123 Main St', passwordHash: 'hashedPassword', patientRecords: [] };
    component.patient = patient;
    component.patientForm.patchValue(patient);
    patientServiceMock.update.and.returnValue(throwError({ error: 'Error updating patient' }));
    spyOn(sessionStorage, 'getItem').and.callFake((key: string) => {
      if (key === 'jwtToken') return 'token';
      return null;
    });

    component.onUpdate();

    expect(console.error).toHaveBeenCalledWith('Error updating patient:', { error: 'Error updating patient' });
  });

  it('should update password', () => {
    const patient: Patient = { id: '1', firstName: 'Jane', lastName: 'Doe', email: 'jane.doe@example.com', phoneNumber: '+1234567890', birthDate: '1990-01-01', gender: 'Female', address: '123 Main St', passwordHash: 'hashedPassword', patientRecords: [] };
    component.patient = patient;
    component.resetPasswordForm.patchValue({ newPassword: 'NewPassword123!', confirmPassword: 'NewPassword123!' });
    patientServiceMock.updatePassword.and.returnValue(of({}));
    spyOn(sessionStorage, 'getItem').and.callFake((key: string) => {
      if (key === 'jwtToken') return 'token';
      return null;
    });

    component.onUpdatePassword();

    expect(patientServiceMock.updatePassword).toHaveBeenCalledWith({ patientId: '1', password: 'NewPassword123!' }, 'token');
    expect(component.passwordRequired).toBeFalse();
  });

  it('should handle error when updating password', () => {
    spyOn(console, 'error');
    const patient: Patient = { id: '1', firstName: 'Jane', lastName: 'Doe', email: 'jane.doe@example.com', phoneNumber: '+1234567890', birthDate: '1990-01-01', gender: 'Female', address: '123 Main St', passwordHash: 'hashedPassword', patientRecords: [] };
    component.patient = patient;
    component.resetPasswordForm.patchValue({ newPassword: 'NewPassword123!', confirmPassword: 'NewPassword123!' });
    patientServiceMock.updatePassword.and.returnValue(throwError({ error: 'Error updating password' }));
    spyOn(sessionStorage, 'getItem').and.callFake((key: string) => {
      if (key === 'jwtToken') return 'token';
      return null;
    });

    component.onUpdatePassword();

    expect(console.error).toHaveBeenCalledWith('Error updating password:', { error: 'Error updating password' });
  });

  it('should set active section', () => {
    component.setActiveSection('editPersonalDetails');
    expect(component.activeSection).toBe('editPersonalDetails');
    expect(component.editMode).toBeFalse();
    expect(component.passwordRequired).toBeFalse();
    expect(component.errorMessage).toBeNull();
  });

  it('should cancel edit', () => {
    component.cancelEdit();
    expect(component.editMode).toBeFalse();
    expect(component.passwordRequired).toBeFalse();
    expect(component.errorMessage).toBeNull();
    expect(component.activeSection).toBe('profile');
  });

  it('should submit form and require password', () => {
    component.patientForm.patchValue({
      firstName: 'Jane',
      lastName: 'Doe',
      birthDate: '1990-01-01',
      gender: 'Female',
      email: 'jane.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St'
    });

    component.onSubmit();

    expect(component.passwordRequired).toBeTrue();
  });

  it('should not submit form if invalid', () => {
    component.patientForm.patchValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: ''
    });

    component.onSubmit();

    expect(component.passwordRequired).toBeFalse();
  });

  it('should reset password form and require current password', () => {
    component.resetPasswordForm.patchValue({
      newPassword: 'NewPassword123!',
      confirmPassword: 'NewPassword123!'
    });

    component.onResetPassword();

    expect(component.passwordRequired).toBeFalse();
  });
});
