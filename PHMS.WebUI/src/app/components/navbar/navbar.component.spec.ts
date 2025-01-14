import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { NavbarComponent } from './navbar.component';
import { By } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';

fdescribe('NavbarComponent', () => {
  let component: NavbarComponent;
  let fixture: ComponentFixture<NavbarComponent>;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [CommonModule, NavbarComponent],
      providers: [
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(NavbarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should check login status on init', () => {
    spyOn(component, 'checkLoginStatus').and.callThrough();
    component.ngOnInit();
    expect(component.checkLoginStatus).toHaveBeenCalled();
  });

  it('should check screen size on init', () => {
    spyOn(component, 'checkScreenSize').and.callThrough();
    component.ngOnInit();
    expect(component.checkScreenSize).toHaveBeenCalled();
  });

  it('should toggle menu visibility', () => {
    component.menuOpen = false;
    component.toggleMenu();
    expect(component.menuOpen).toBeTrue();

    component.toggleMenu();
    expect(component.menuOpen).toBeFalse();
  });

  it('should check screen size and hide menu on larger screens', () => {
    window.innerWidth = 1024;
    component.checkScreenSize();
    expect(component.isSmallScreen).toBeFalse();
    expect(component.menuOpen).toBeFalse();
  });

  it('should check screen size and not hide menu on small screens', () => {
    window.innerWidth = 600;
    component.checkScreenSize();
    expect(component.isSmallScreen).toBeTrue();
  });

  it('should update isSmallScreen when window is resized', () => {
    window.innerWidth = 600;
    component.checkScreenSize();
    expect(component.isSmallScreen).toBeTrue();
    
    window.innerWidth = 1024;
    component.checkScreenSize();
    expect(component.isSmallScreen).toBeFalse();
  });

  it('should set login status based on session storage', () => {
    sessionStorage.setItem('jwtToken', 'testToken');
    sessionStorage.setItem('userId', 'testUser');
    sessionStorage.setItem('role', 'Patient');

    component.checkLoginStatus();

    expect(component.isLoggedIn).toBeTrue();
    expect(component.isPatient).toBeTrue();
    expect(component.isMedic).toBeFalse();
  });

  it('should set medic role based on session storage', () => {
    sessionStorage.setItem('jwtToken', 'testToken');
    sessionStorage.setItem('userId', 'testUser');
    sessionStorage.setItem('role', 'Medic');

    component.checkLoginStatus();

    expect(component.isLoggedIn).toBeTrue();
    expect(component.isPatient).toBeFalse();
    expect(component.isMedic).toBeTrue();
  });

  it('should logout correctly', () => {
    sessionStorage.setItem('jwtToken', 'testToken');
    sessionStorage.setItem('userId', 'testUser');
    sessionStorage.setItem('role', 'Patient');

    component.logout();

    expect(sessionStorage.getItem('jwtToken')).toBeNull();
    expect(sessionStorage.getItem('userId')).toBeNull();
    expect(sessionStorage.getItem('role')).toBeNull();
    expect(component.isLoggedIn).toBeFalse();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/']);
  });

  it('should redirect to chat', () => {
    component.redirectToGetChat();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/chat']);
  });

  it('should redirect to medics', () => {
    component.redirectToGetMedics();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/medics/paginated']);
  });

  it('should redirect to specialties', () => {
    component.redirectToGetSpecializations();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/specialties']);
  });

  it('should redirect to consultations', () => {
    component.redirectToGetConsultations();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/consultations']);
  });

  it('should redirect to login', () => {
    component.redirectToLogin();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should redirect to patient list', () => {
    component.redirectToPatientList();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/patients']);
  });

  it('should redirect to my details with user id', () => {
    sessionStorage.setItem('userId', 'testUser');
    component.redirectToMyDetails();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/patients/testUser']);
  });

  it('should not redirect to my details if user id is missing', () => {
    sessionStorage.removeItem('userId');
    component.redirectToMyDetails();
    expect(routerSpy.navigate).not.toHaveBeenCalled();
  });

  it('should show links for logged-in patient', () => {
    component.isLoggedIn = true;
    component.isPatient = true;
    fixture.detectChanges();
    
    const links = fixture.debugElement.queryAll(By.css('.navbar-link'));
    expect(links.length).toBeGreaterThan(0);
  });

  it('should show links for logged-in medic', () => {
    component.isLoggedIn = true;
    component.isMedic = true;
    fixture.detectChanges();
    
    const links = fixture.debugElement.queryAll(By.css('.navbar-link'));
    expect(links.length).toBeGreaterThan(0);
  });

  it('should show login link when not logged in', () => {
    component.isLoggedIn = false;
    fixture.detectChanges();

    const loginLink = fixture.debugElement.query(By.css('.navbar-link:last-child'));
    expect(loginLink.nativeElement.textContent).toContain('LOGIN');
  });

  
});