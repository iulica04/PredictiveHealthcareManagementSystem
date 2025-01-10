import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SpecializationsComponent } from './specializations.component';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { By } from '@angular/platform-browser';

fdescribe('SpecializationsComponent', () => {
  let component: SpecializationsComponent;
  let fixture: ComponentFixture<SpecializationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [],
      imports: [CommonModule, NavbarComponent, SpecializationsComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(SpecializationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should have a list of specializations', () => {
    expect(component.specializations).toBeDefined();
    expect(component.specializations.length).toBeGreaterThan(0);
  });

  it('should select a specialization and display details', () => {
    const testSpecialization = 'Cardiology';
    const testDetails = 'Cardiology is the branch of medicine that deals with diseases and abnormalities of the heart. Common conditions include heart failure, coronary artery disease, and arrhythmias.';

    component.filterBySpecialization({ target: { value: testSpecialization } } as unknown as Event);

    expect(component.selectedSpecialization).toBe(testSpecialization);
    expect(component.specializationDetails).toBe(testDetails);
  });

  it('should return default message for unknown specialization', () => {
    const testSpecialization = 'UnknownSpecialization';

    component.filterBySpecialization({ target: { value: testSpecialization } } as unknown as Event);

    expect(component.selectedSpecialization).toBe(testSpecialization);
    expect(component.specializationDetails).toBe('Please select a specialization to see the details.');
  });

  it('should render and update specialization details correctly', () => {

    expect(component.specializations).toContain('Cardiology');
    const selectElement = fixture.debugElement.query(By.css('select'));
    selectElement.triggerEventHandler('change', { target: { value: 'Cardiology' } });
    fixture.detectChanges();
    const detailsElement = fixture.debugElement.query(By.css('.details'));
    expect(detailsElement.nativeElement.textContent).toContain(
      'Cardiology is the branch of medicine that deals with diseases and abnormalities of the heart'
    );
  });

  it('should update the displayed details when a specialization is selected', () => {
    const selectElement = fixture.debugElement.query(By.css('select'));
    selectElement.triggerEventHandler('change', { target: { value: 'Neurology' } });

    fixture.detectChanges();

    const detailsElement = fixture.debugElement.query(By.css('.details'));
    expect(detailsElement.nativeElement.textContent).toContain('Neurology focuses on the diagnosis and treatment of disorders of the nervous system');
  });
});