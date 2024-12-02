import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicDetailComponent } from './medic-detail.component';

describe('MedicDetailComponent', () => {
  let component: MedicDetailComponent;
  let fixture: ComponentFixture<MedicDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MedicDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
