import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MedicGetAllComponent } from './medic-get-all.component';
import { MedicService } from '../../services/medic.service';
import { of, throwError } from 'rxjs';
import { CommonModule } from '@angular/common';

describe('MedicGetAllComponent', () => {
  let component: MedicGetAllComponent;
  let fixture: ComponentFixture<MedicGetAllComponent>;
  let medicServiceMock: any;

  beforeEach(async () => {
    medicServiceMock = jasmine.createSpyObj('MedicService', ['getAll']);

    medicServiceMock.getAll.and.returnValue(of({
      data: [
        {
          id: '1',
          firstName: 'John',
          specialization: 'Cardiology',
          rank: 'Senior',
          email: 'john.doe@example.com'
        },
        {
          id: '2',
          firstName: 'Jane',
          specialization: 'Neurology',
          rank: 'Junior',
          email: 'jane.smith@example.com'
        }
      ],
      totalCount: 2
    }));

    await TestBed.configureTestingModule({
      imports: [CommonModule],
      declarations: [MedicGetAllComponent],
      providers: [
        { provide: MedicService, useValue: medicServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MedicGetAllComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load medics on initialization', () => {
    expect(medicServiceMock.getAll).toHaveBeenCalledWith(1, 12, '', '');
    expect(component.medics.length).toBe(2);
    expect(component.medics[0].firstName).toBe('John');
  });

  it('should filter medics by specialization', () => {
    // Arrange: Create a mock event
    const mockEvent = {
      target: { value: 'Cardiology' }
    } as unknown as Event;
  
    // Act: Call the method with the mock event
    component.filterBySpecialization(mockEvent);
  
    // Assert: Ensure the selected specialization is updated and loadMedics is called
    expect(component.selectedSpecialization).toBe('Cardiology');
    expect(component.currentPage).toBe(1);
    expect(medicServiceMock.getAll).toHaveBeenCalledWith(1, 12, '', 'Cardiology');
  });

  it('should go to the next page when nextPage is called', () => {
    component.nextPage();
    fixture.detectChanges();

    expect(component.currentPage).toBe(2);
    expect(medicServiceMock.getAll).toHaveBeenCalledWith(2, 12, '', '');
  });

  it('should go back to the previous page when previousPage is called', () => {
    component.currentPage = 2;
    component.previousPage();
    fixture.detectChanges();

    expect(component.currentPage).toBe(1);
    expect(medicServiceMock.getAll).toHaveBeenCalledWith(1, 12, '', '');
  });

  it('should disable the back button on the first page', () => {
    component.currentPage = 1;
    fixture.detectChanges();

    const compiled = fixture.nativeElement;
    const backButton = compiled.querySelector('button[disabled]');
    expect(backButton.textContent).toContain('Back');
  });

  it('should handle error when loading medics', () => {
    medicServiceMock.getAll.and.returnValue(throwError('Error fetching medics'));
    component.loadMedics();
    fixture.detectChanges();

    expect(component.medics.length).toBe(0);
  });
});
