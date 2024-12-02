import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicCreateComponent } from './medic-create.component';

describe('MedicCreateComponent', () => {
  let component: MedicCreateComponent;
  let fixture: ComponentFixture<MedicCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicCreateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MedicCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
