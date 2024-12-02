import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { MedicUpdateComponent } from './medic-update.component';
import { MedicService } from '../../services/medic.service';
import { CommonModule } from '@angular/common';

fdescribe('MedicUpdateComponent', () => {
  let component: MedicUpdateComponent;
  let fixture: ComponentFixture<MedicUpdateComponent>;
  let medicServiceMock: any;
  let routerMock: any;
  let activatedRouteMock: any;

  beforeEach(async () => {
    medicServiceMock = jasmine.createSpyObj('MedicService', ['getById', 'update']);
    medicServiceMock.getById.and.returnValue(of({
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'Hospital'
    }));
    medicServiceMock.update.and.returnValue(of({}));

    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy().and.returnValue('1')
        }
      }
    };

    await TestBed.configureTestingModule({
      imports: [CommonModule, ReactiveFormsModule, MedicUpdateComponent],
      providers: [
        { provide: MedicService, useValue: medicServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MedicUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize the form with empty password', () => {
    expect(component.medicForm).toBeTruthy();
    expect(component.medicForm.get('firstName')?.value).toBe('John');
    expect(component.medicForm.get('lastName')?.value).toBe('Doe');
    expect(component.medicForm.get('rank')?.value).toBe('Senior');
    expect(component.medicForm.get('specialization')?.value).toBe('Cardiology');
    expect(component.medicForm.get('hospital')?.value).toBe('Hospital');
    expect(component.medicForm.get('password')?.value).toBe('');
    expect(component.medicForm.valid).toBeFalse(); 
  });

  it('should load medic data on init', () => {
    expect(medicServiceMock.getById).toHaveBeenCalledWith('1');
    expect(component.medicForm.get('firstName')?.value).toBe('John');
    expect(component.medicForm.get('lastName')?.value).toBe('Doe');
  });

  it('should navigate to /medics on error loading medic data', () => {
    medicServiceMock.getById.and.returnValue(throwError('Error loading medic data'));
    component.loadMedicData();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/medics']);
  });

  it('should call update and navigate to /medics on valid form submission', () => {
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

    expect(medicServiceMock.update).toHaveBeenCalledWith('1', {
      id: '1',
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
    expect(routerMock.navigate).toHaveBeenCalledWith(['/medics']);
  });

  it('should not call update if form is invalid', () => {
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

    expect(medicServiceMock.update).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });
});