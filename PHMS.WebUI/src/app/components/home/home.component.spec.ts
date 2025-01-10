import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HomeComponent } from './home.component';
import { RouterTestingModule } from '@angular/router/testing';
import { Router } from '@angular/router';
import { By } from '@angular/platform-browser';

fdescribe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;
  let routerMock: any;

  beforeEach(async () => {
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [RouterTestingModule, HomeComponent],
      providers: [
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display welcome message', () => {
    const compiled = fixture.debugElement.nativeElement;
    expect(compiled.querySelector('h1').textContent).toContain('Welcome to our website!');
  });

  it('should display appointment scheduling section', () => {
    const compiled = fixture.debugElement.nativeElement;
    const section = compiled.querySelector('.box:nth-child(1)');
    expect(section.querySelector('h2').textContent).toContain('Easy Appointment Scheduling');
  });

  it('should display doctor communication section', () => {
    const compiled = fixture.debugElement.nativeElement;
    const section = compiled.querySelector('.box:nth-child(2)');
    expect(section.querySelector('h2').textContent).toContain('Stay Connected with Your Doctor');
  });

  it('should display AI-powered treatments section', () => {
    const compiled = fixture.debugElement.nativeElement;
    const section = compiled.querySelector('.box:nth-child(3)');
    expect(section.querySelector('h2').textContent).toContain('AI-Powered Treatments & Diagnostics');
  });

  it('should navigate to login on redirectToLogin call', () => {
    component.redirectToLogin();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should navigate to login when login link is clicked', () => {
    const loginLink = fixture.debugElement.query(By.css('.navbar-link.login-link')).nativeElement;
    loginLink.click();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });
});
