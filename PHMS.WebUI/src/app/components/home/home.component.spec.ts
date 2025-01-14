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

  

  it('should display all images with correct src and alt attributes', () => {
    const compiled = fixture.debugElement.nativeElement;
    const images = compiled.querySelectorAll('.box img');
    expect(images.length).toBe(3);
    expect(images[0].src).toContain('assets/images/image8.png');
    expect(images[0].alt).toBe('Image 3');
    expect(images[1].src).toContain('assets/images/image7.png');
    expect(images[1].alt).toBe('Image 2');
    expect(images[2].src).toContain('assets/images/image6.png');
    expect(images[2].alt).toBe('Image 1');
  });

  it('should display all descriptions with correct text', () => {
    const compiled = fixture.debugElement.nativeElement;
    const descriptions = compiled.querySelectorAll('.description p');
    expect(descriptions.length).toBe(3);
    expect(descriptions[0].textContent).toContain('With our app, scheduling your next medical appointment is easier than ever. Just a few clicks and you’re set!');
    expect(descriptions[1].textContent).toContain('Through our app, you can easily communicate with your doctor, get updates on your treatment, and receive personalized advice whenever you need it.');
    expect(descriptions[2].textContent).toContain('Our platform utilizes the latest medical technologies, including AI, to offer accurate diagnostics and personalized treatment plans for your health needs.');
  });

  it('should have a navbar component', () => {
    const navbar = fixture.debugElement.query(By.css('app-navbar'));
    expect(navbar).toBeTruthy();
  });

  it('should have a main page container', () => {
    const mainPage = fixture.debugElement.query(By.css('.main-page'));
    expect(mainPage).toBeTruthy();
  });
});