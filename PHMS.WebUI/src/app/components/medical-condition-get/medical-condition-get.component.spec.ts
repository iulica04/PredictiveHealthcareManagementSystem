import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalConditionGetComponent } from './medical-condition-get.component';

describe('MedicalConditionGetComponent', () => {
  let component: MedicalConditionGetComponent;
  let fixture: ComponentFixture<MedicalConditionGetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicalConditionGetComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MedicalConditionGetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
