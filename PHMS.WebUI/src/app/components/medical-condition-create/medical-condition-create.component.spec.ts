import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalConditionCreateComponent } from './medical-condition-create.component';

describe('MedicalConditionCreateComponent', () => {
  let component: MedicalConditionCreateComponent;
  let fixture: ComponentFixture<MedicalConditionCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicalConditionCreateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MedicalConditionCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
