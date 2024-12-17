import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MedicListComponent } from './medic-list.component';
import { MedicService } from '../../services/medic.service';
import { of } from 'rxjs';
import { Router } from '@angular/router';
import { Medic } from '../../models/medic.model';
import { CommonModule } from '@angular/common';

describe('MedicListComponent', () => {
  let component: MedicListComponent;
  let fixture: ComponentFixture<MedicListComponent>;
  let medicServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    // Mock MedicService
    medicServiceMock = jasmine.createSpyObj('MedicService', ['getMedics']);
    medicServiceMock.getMedics.and.returnValue(of([
      {
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
        hospital: 'General Hospital'
      },
      {
        id: '2',
        firstName: 'Jane',
        lastName: 'Smith',
        birthDate: '1995-02-10',
        gender: 'Female',
        email: 'jane.smith@example.com',
        phoneNumber: '+0987654321',
        address: '456 Oak St',
        rank: 'Junior',
        specialization: 'Neurology',
        hospital: 'City Hospital'
      }
    ]));

    // Mock Router
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [CommonModule, MedicListComponent],
      providers: [
        { provide: MedicService, useValue: medicServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MedicListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load medics on init', () => {
    expect(medicServiceMock.getMedics).toHaveBeenCalled();
    expect(component.medics.length).toBe(2);
    expect(component.medics[0].firstName).toBe('John');
    expect(component.medics[1].firstName).toBe('Jane');
  });

  it('should navigate to create medic page on navigateToCreateMedic', () => {
    component.navigateToCreateMedic();
    expect(routerMock.navigate).toHaveBeenCalledWith(['medics/create']);
  });

  it('should navigate to medic detail page on navigateToDetailMedic', () => {
    component.navigateToDetailMedic('1');
    expect(routerMock.navigate).toHaveBeenCalledWith(['medics/1']);
  });

  it('should navigate to update medic page on navigateToUpdateMedic', () => {
    component.navigateToUpdateMedic('1');
    expect(routerMock.navigate).toHaveBeenCalledWith(['medics/update/1']);
  });

  it('should navigate to get all filtered medics page on navigateToPaginatedMedics', () => {
    component.navigateToPaginatedMedics();
    expect(routerMock.navigate).toHaveBeenCalledWith(['medics/paginated']);
  });
});