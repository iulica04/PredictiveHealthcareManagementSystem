import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MedicListComponent } from './medic-list.component';
import { MedicService } from '../../services/medic.service';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Medic } from '../../models/medic.model';

fdescribe('MedicListComponent', () => {
  let component: MedicListComponent;
  let fixture: ComponentFixture<MedicListComponent>;
  let medicServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    medicServiceMock = jasmine.createSpyObj('MedicService', ['getMedics', 'logout']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [CommonModule, MedicListComponent],  // Import MedicListComponent here
      providers: [
        { provide: MedicService, useValue: medicServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MedicListComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize medics on ngOnInit', () => {
    const mockMedics: Medic[] = [
      { 
        id: '1',
        firstName: 'John', 
        lastName: 'Doe', 
        birthDate: '1990-01-01', 
        gender: 'Male', 
        email: 'john@example.com', 
        passwordHash: 'hash123', 
        phoneNumber: '1234567890', 
        address: '123 Main St', 
        rank: 'Senior', 
        specialization: 'Cardiology', 
        hospital: 'City Hospital' 
      }
    ];
    medicServiceMock.getMedics.and.returnValue(of(mockMedics));

    component.ngOnInit();

    expect(component.medics).toEqual(mockMedics);
    expect(medicServiceMock.getMedics).toHaveBeenCalled();
  });

  it('should navigate to create medic', () => {
    component.navigateToCreateMedic();
    expect(routerMock.navigate).toHaveBeenCalledWith(['medics/create']);
  });

  it('should navigate to detail medic', () => {
    const id = '1';
    component.navigateToDetailMedic(id);
    expect(routerMock.navigate).toHaveBeenCalledWith([`medics/${id}`]);
  });

  it('should navigate to update medic', () => {
    const id = '1';
    component.navigateToUpdateMedic(id);
    expect(routerMock.navigate).toHaveBeenCalledWith([`medics/update/${id}`]);
  });

  it('should navigate to paginated medics', () => {
    component.navigateToPaginatedMedics();
    expect(routerMock.navigate).toHaveBeenCalledWith(['medics/paginated']);
  });

  it('should logout', () => {
    component.logout();
    expect(medicServiceMock.logout).toHaveBeenCalled();
  });
});
