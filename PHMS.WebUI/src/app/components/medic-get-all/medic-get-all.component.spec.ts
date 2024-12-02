import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MedicGetAllComponent } from './medic-get-all.component';
import { MedicService } from '../../services/medic.service';
import { of, throwError } from 'rxjs';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Medic } from '../../models/medic.model';

fdescribe('MedicGetAllComponent', () => {
  let component: MedicGetAllComponent;
  let fixture: ComponentFixture<MedicGetAllComponent>;
  let medicServiceMock: any;

  beforeEach(async () => {
    // Mock MedicService
    medicServiceMock = jasmine.createSpyObj('MedicService', ['getAll']);
    medicServiceMock.getAll.and.returnValue(of({
      data: [
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
      ],
      totalCount: 2
    }));

    await TestBed.configureTestingModule({
      imports: [CommonModule, ReactiveFormsModule, MedicGetAllComponent],
      providers: [
        { provide: MedicService, useValue: medicServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MedicGetAllComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load medics on form submit', () => {
    component.onSubmit();
    fixture.detectChanges(); // Ensure change detection is run
    expect(medicServiceMock.getAll).toHaveBeenCalled();
    expect(component.medics.length).toBe(2);
    expect(component.medics[0].firstName).toBe('John');
    expect(component.medics[1].firstName).toBe('Jane');
  });


  it('should submit form and get filtered medics', () => {
    component.medicForm.setValue({ page: 1, pageSize: 10, rank: 'Senior', specialization: 'Cardiology' });
    component.onSubmit();
    fixture.detectChanges(); // Ensure change detection is run
    expect(medicServiceMock.getAll).toHaveBeenCalledWith(1, 10, 'Senior', 'Cardiology');
  });

  it('should update form values correctly', () => {
    component.medicForm.setValue({ page: 2, pageSize: 20, rank: 'Junior', specialization: 'Neurology' });
    expect(component.medicForm.value).toEqual({ page: 2, pageSize: 20, rank: 'Junior', specialization: 'Neurology' });
  });

  it('should initialize form with default values', () => {
    expect(component.medicForm.value).toEqual({ page: 1, pageSize: 10, rank: '', specialization: '' });
  });

  it('should handle error when fetching medics', () => {
    medicServiceMock.getAll.and.returnValue(throwError('Error fetching medics'));
    component.onSubmit();
    fixture.detectChanges();
    expect(component.medics.length).toBe(0);
    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('.error-message').textContent).toContain('Error fetching medics');
  });

});