import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MedicalConditionListComponent } from './medical-condition-list.component';
import { MedicalConditionService } from '../../services/medical-condition.service';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';

fdescribe('MedicalConditionListComponent', () => {
  let component: MedicalConditionListComponent;
  let fixture: ComponentFixture<MedicalConditionListComponent>;
  let medicalConditionService: jasmine.SpyObj<MedicalConditionService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const medicalConditionServiceSpy = jasmine.createSpyObj('MedicalConditionService', ['getMedicalConditionsByPatientId']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      declarations: [MedicalConditionListComponent, NavbarComponent],
      imports: [CommonModule, HttpClientTestingModule],
      providers: [
        { provide: MedicalConditionService, useValue: medicalConditionServiceSpy },
        { provide: Router, useValue: routerSpy },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: { paramMap: { get: () => '123' } }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MedicalConditionListComponent);
    component = fixture.componentInstance;
    medicalConditionService = TestBed.inject(MedicalConditionService) as jasmine.SpyObj<MedicalConditionService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;

    medicalConditionService.getMedicalConditionsByPatientId.and.returnValue(of([]));
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch medical conditions on init', () => {
    expect(medicalConditionService.getMedicalConditionsByPatientId).toHaveBeenCalledWith('123');
  });

  it('should navigate to create medical condition', () => {
    component.navigateToCreateMedicalCondition();
    expect(router.navigate).toHaveBeenCalledWith(['/medical-condition-create/123']);
  });


  it('should display "No medical conditions found for this patient." when there are no medical conditions', () => {
    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('p').textContent).toContain('No medical conditions found for this patient.');
  });

  it('should display medical conditions when they are available', () => {
    const mockConditions: MedicalCondition[] = [
      {
        medicalConditionId: '10d62ddd-a734-4d3e-9083-aedb446cf31f', // Adaugă acest câmp
        patientId: '10d62ddd-a734-4d3e-9083-aedb446cf31e', // Adaugă acest câmp
        name: 'Condition 1',
        description: 'Description 1',
        startDate: '2023-01-01T00:00:00Z',
        currentStatus: 'ongoing',
        isGenetic: true,
        recommendation: 'Recommendation 1',
        treatments: [
          {
            name: 'Treatment 1',
            type: 0,
            location: 'Location 1',
            startDate: '2023-01-01T00:00:00Z',
            duration: '2023-01-02T00:00:00Z',
            frequency: 'Daily',
            medications: [
              {
                name: 'Medication 1',
                type: 0,
                ingredients: 'Ingredients 1',
                adverseEffects: 'Adverse Effects 1',
              },
            ],
          },
        ],
      },
    ];

    medicalConditionService.getMedicalConditionsByPatientId.and.returnValue(of(mockConditions));
    component.ngOnInit();
    fixture.detectChanges();

    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('.condition-title h3').textContent).toContain('Condition 1');
    expect(compiled.querySelector('.description').textContent).toContain('Description 1');
    expect(compiled.querySelector('.status-genetic p').textContent).toContain('ongoing');
    expect(compiled.querySelector('.status-genetic p').textContent).toContain('Yes');
    expect(compiled.querySelector('.treatment-details p').textContent).toContain('Treatment 1');
    expect(compiled.querySelector('.medication p').textContent).toContain('Medication 1');
  });
});