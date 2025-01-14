import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicalConditionListComponent } from './medical-condition-list.component';

describe('MedicalConditionListComponent', () => {
  let component: MedicalConditionListComponent;
  let fixture: ComponentFixture<MedicalConditionListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicalConditionListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MedicalConditionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
