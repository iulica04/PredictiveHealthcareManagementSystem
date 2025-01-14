import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalConditionUpdateComponent } from './medical-condition-update.component';

describe('MedicalConditionUpdateComponent', () => {
  let component: MedicalConditionUpdateComponent;
  let fixture: ComponentFixture<MedicalConditionUpdateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicalConditionUpdateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MedicalConditionUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
