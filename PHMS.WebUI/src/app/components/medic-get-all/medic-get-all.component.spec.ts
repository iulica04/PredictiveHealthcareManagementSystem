import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicGetAllComponent } from './medic-get-all.component';

describe('MedicGetAllComponent', () => {
  let component: MedicGetAllComponent;
  let fixture: ComponentFixture<MedicGetAllComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicGetAllComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MedicGetAllComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
