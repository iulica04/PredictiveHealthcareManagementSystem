import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PatientGetAllComponent } from './patient-get-all.component';

describe('PatientGetAllComponent', () => {
  let component: PatientGetAllComponent;
  let fixture: ComponentFixture<PatientGetAllComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PatientGetAllComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PatientGetAllComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
