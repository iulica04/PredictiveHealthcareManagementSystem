import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicUpdateComponent } from './medic-update.component';

describe('MedicUpdateComponent', () => {
  let component: MedicUpdateComponent;
  let fixture: ComponentFixture<MedicUpdateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicUpdateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MedicUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
