import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NavbarComponent } from './navbar.component';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { By } from '@angular/platform-browser';

fdescribe('NavbarComponent', () => {
  let component: NavbarComponent;
  let fixture: ComponentFixture<NavbarComponent>;
  let router: Router;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, NavbarComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(NavbarComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to login when redirectToLogin is called', () => {
    const navigateSpy = spyOn(router, 'navigate');
    component.redirectToLogin();
    expect(navigateSpy).toHaveBeenCalledWith(['/login']);
  });

  it('should navigate to medics when redirectToGetMedics is called', () => {
    const navigateSpy = spyOn(router, 'navigate');
    component.redirectToGetMedics();
    expect(navigateSpy).toHaveBeenCalledWith(['/medics/paginated']);
  });

  it('should navigate to specializations when redirectToGetSpecializations is called', () => {
    const navigateSpy = spyOn(router, 'navigate');
    component.redirectToGetSpecializations();
    expect(navigateSpy).toHaveBeenCalledWith(['/specialties']);
  });

  it('should call redirectToLogin when login link is clicked', () => {
    const redirectToLoginSpy = spyOn(component, 'redirectToLogin');
    const loginLink = fixture.debugElement.query(By.css('.login-link')).nativeElement;
    loginLink.click();
    expect(redirectToLoginSpy).toHaveBeenCalled();
  });

  it('should call redirectToGetMedics when medics link is clicked', () => {
    const redirectToGetMedicsSpy = spyOn(component, 'redirectToGetMedics');
    const medicsLink = fixture.debugElement.query(By.css('.navbar-link:nth-child(1)')).nativeElement;
    medicsLink.click();
    expect(redirectToGetMedicsSpy).toHaveBeenCalled();
  });

  it('should call redirectToGetSpecializations when specializations link is clicked', () => {
    const redirectToGetSpecializationsSpy = spyOn(component, 'redirectToGetSpecializations');
    const specializationsLink = fixture.debugElement.query(By.css('.navbar-link:nth-child(2)')).nativeElement;
    specializationsLink.click();
    expect(redirectToGetSpecializationsSpy).toHaveBeenCalled();
  });
});