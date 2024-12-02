import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicListComponent } from './medic-list.component';

describe('MedicListComponent', () => {
  let component: MedicListComponent;
  let fixture: ComponentFixture<MedicListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MedicListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MedicListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
