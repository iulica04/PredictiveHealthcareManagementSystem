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
      hospital: 'General Hospital',
      passwordHash: 'hashedpassword'
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
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load medic data on init', () => {
    fixture.detectChanges();
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
    fixture.detectChanges();
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      password: 'Password1!',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
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
      password: 'Password1!',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
    });
    expect(routerMock.navigate).toHaveBeenCalledWith(['/medics']);
  });

  it('should not load medic data if medicId is missing on route', () => {
    activatedRouteMock.snapshot.paramMap.get.and.returnValue(null);
    component.ngOnInit();
    expect(medicServiceMock.getById).not.toHaveBeenCalled();
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
      password: '',
      rank: '',
      specialization: '',
      hospital: ''
    });

    component.onSubmit();

    expect(medicServiceMock.update).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('should log an error and not navigate on error updating medic', () => {
    fixture.detectChanges();
    const consoleSpy = spyOn(console, 'error');
    medicServiceMock.update.and.returnValue(throwError('Error updating medic'));

    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      password: 'Password1!',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
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
      password: 'Password1!',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'General Hospital'
    });
    expect(consoleSpy).toHaveBeenCalledWith('Error updating medic:', 'Error updating medic');
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });
});