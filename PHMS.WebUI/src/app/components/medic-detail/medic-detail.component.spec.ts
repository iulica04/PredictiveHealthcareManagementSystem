import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MedicDetailComponent } from './medic-detail.component';
import { MedicService } from '../../services/medic.service';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { CommonModule } from '@angular/common';
import { RouterTestingModule } from '@angular/router/testing';

fdescribe('MedicDetailComponent', () => {
  let component: MedicDetailComponent;
  let fixture: ComponentFixture<MedicDetailComponent>;
  let medicServiceMock: any;
  let routerMock: any;
  let activatedRouteMock: any;

  beforeEach(async () => {
    // Mock MedicService
    medicServiceMock = jasmine.createSpyObj('MedicService', ['getById', 'delete', 'logout']);
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

    // Mock Router
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    // Mock ActivatedRoute
    activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.callFake((key: string) => {
            if (key === 'id') {
              return '1';
            }
            return null;
          })
        }
      }
    };

    await TestBed.configureTestingModule({
      imports: [CommonModule, RouterTestingModule],
      providers: [
        { provide: MedicService, useValue: medicServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MedicDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  beforeEach(() => {
    sessionStorage.setItem('jwtToken', 'mockToken'); // Setează token-ul în sessionStorage
  });

  afterEach(() => {
    sessionStorage.removeItem('jwtToken'); // Curăță token-ul din sessionStorage
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load medic details on init', () => {
    expect(medicServiceMock.getById).toHaveBeenCalledWith('1');
    expect(component.medic).toEqual({
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
    });
  });

  it('should display loading message when medic details are not yet loaded', () => {
    component.medic = undefined;
    fixture.detectChanges();
    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('p').textContent).toContain('Loading medic details...');
  });

  it('should handle error when fetching medic details', () => {
    medicServiceMock.getById.and.returnValue(throwError(() => new Error('Error fetching medic details')));
    component.ngOnInit();
    fixture.detectChanges();
    const compiled = fixture.nativeElement;
    expect(component.medic).toBeUndefined();
    expect(compiled.querySelector('p').textContent).toContain('Loading medic details...');
  });

  it('should call deleteMedic and navigate to medics list on delete', () => {
    medicServiceMock.delete.and.returnValue(of({}));

    // Setează un token fals în sessionStorage
    sessionStorage.setItem('jwtToken', 'mockToken');

    // Setează id-ul medicului
    component.medic = {
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
    };

    component.deleteMedic();
    fixture.detectChanges();
    expect(medicServiceMock.delete).toHaveBeenCalledWith('1', 'mockToken');
    expect(routerMock.navigate).toHaveBeenCalledWith(['medics']);
  });

  it('should handle error when deleting medic', () => {
    medicServiceMock.delete.and.returnValue(throwError(() => new Error('Error deleting medic')));
    
    component.medic = {
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
    };

    // Setează un token fals în sessionStorage
    sessionStorage.setItem('jwtToken', 'mockToken');
    component.deleteMedic();
    fixture.detectChanges();
    expect(medicServiceMock.delete).toHaveBeenCalledWith('1', 'mockToken');
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('should not call deleteMedic if medic is undefined', () => {
    medicServiceMock.delete.and.returnValue(of({})); // Asigură-te că returnează un Observable

    component.medic = undefined;
    component.deleteMedic();
    expect(medicServiceMock.delete).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('should navigate to update medic page on update button click', () => {
    component.medic = { id: '1', firstName: 'John', lastName: 'Doe', birthDate: '2000-01-01', gender: 'Male', email: 'john.doe@example.com', phoneNumber: '+1234567890', address: '123 Main St', rank: 'Senior', specialization: 'Cardiology', hospital: 'General Hospital', passwordHash: 'hashedpassword' };
    component.navigateToUpdateMedic('1');
    expect(routerMock.navigate).toHaveBeenCalledWith(['medics/update/1']);
  });

  it('should handle error when loading medic details', () => {
    medicServiceMock.getById.and.returnValue(throwError(() => new Error('Error loading medic details')));
    
    component.ngOnInit();
    fixture.detectChanges();

    expect(medicServiceMock.getById).toHaveBeenCalledWith('1');
    expect(component.medic).toBeUndefined();
  });
});