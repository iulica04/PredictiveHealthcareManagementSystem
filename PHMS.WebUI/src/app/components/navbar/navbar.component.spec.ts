import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NavbarComponent } from './navbar.component';
import { RouterTestingModule } from '@angular/router/testing';
import { By } from '@angular/platform-browser';

describe('NavbarComponent', () => {
  let component: NavbarComponent;
  let fixture: ComponentFixture<NavbarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, NavbarComponent] // Mută NavbarComponent în imports
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NavbarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have a logo', () => {
    const logo = fixture.debugElement.query(By.css('.navbar-logo')).nativeElement;
    expect(logo).toBeTruthy();
  });

  it('should have a search input', () => {
    const searchInput = fixture.debugElement.query(By.css('.navbar-input')).nativeElement;
    expect(searchInput).toBeTruthy();
  });

  it('should navigate to medical team on click', () => {
    spyOn(component, 'redirectToGetMedics');
    const medicalTeamLink = fixture.debugElement.queryAll(By.css('.navbar-link'))[0].nativeElement;
    medicalTeamLink.click();
    expect(component.redirectToGetMedics).toHaveBeenCalled();
  });

  it('should navigate to specializations on click', () => {
    spyOn(component, 'redirectToGetSpecializations');
    const link = fixture.debugElement.queryAll(By.css('.navbar-link'))[1].nativeElement;
    link.click();
    expect(component.redirectToGetSpecializations).toHaveBeenCalled();
  });

  it('should navigate to login on click', () => {
    spyOn(component, 'redirectToLogin');
    const link = fixture.debugElement.queryAll(By.css('.navbar-link'))[3].nativeElement;
    link.click();
    expect(component.redirectToLogin).toHaveBeenCalled();
  });
});