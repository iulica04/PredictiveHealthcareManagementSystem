import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { MedicCreateComponent } from './medic-create.component';
import { MedicService } from '../../services/medic.service';
import { CommonModule } from '@angular/common';

describe('MedicCreateComponent', () => {
  let component: MedicCreateComponent;
  let fixture: ComponentFixture<MedicCreateComponent>;
  let medicServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    medicServiceMock = jasmine.createSpyObj('MedicService', ['createMedic']);
    medicServiceMock.createMedic.and.returnValue(of({}));

    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, CommonModule, MedicCreateComponent],
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

  it('should initialize the form with empty values', () => {
    const form = component.medicForm;
    expect(form).toBeTruthy();
    expect(form.get('firstName')?.value).toBe('');
    expect(form.get('lastName')?.value).toBe('');
    expect(form.get('rank')?.value).toBe('');
    expect(form.valid).toBeFalse();
  });

  it('should mark the form as valid with correct values', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      rank: 'Senior', 
      specialization: 'Cardiology',
      hospital: 'Hospital',
      password: 'Password1!'
    });
    expect(component.medicForm.valid).toBeTrue();
  });

  it('should call MedicService.createMedic and navigate on valid form submission', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'Hospital',
      password: 'Password1!'
    });

    component.onSubmit();

    expect(medicServiceMock.createMedic).toHaveBeenCalledWith(component.medicForm.value);
    expect(routerMock.navigate).toHaveBeenCalledWith(['/medics']);
  });

  it('should not call MedicService.createMedic if form is invalid', () => {
    component.medicForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      rank: '', 
      specialization: '',
      hospital: '',
      password: ''
    });

    component.onSubmit();

    expect(medicServiceMock.createMedic).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });
});