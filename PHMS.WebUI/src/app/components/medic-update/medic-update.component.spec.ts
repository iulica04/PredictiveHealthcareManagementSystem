import { TestBed, ComponentFixture } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { MedicUpdateComponent } from './medic-update.component';
import { MedicService } from '../../services/medic.service';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { Medic } from '../../models/medic.model';

fdescribe('MedicUpdateComponent', () => {
  let component: MedicUpdateComponent;
  let fixture: ComponentFixture<MedicUpdateComponent>;
  let medicServiceMock: any;
  let activatedRouteMock: any;
  let routerMock: any;

  beforeEach(async () => {
    medicServiceMock = jasmine.createSpyObj('MedicService', ['getById', 'update']);
    activatedRouteMock = { snapshot: { paramMap: { get: () => '1' } } };
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, MedicUpdateComponent],  // Import MedicUpdateComponent here
      providers: [
        { provide: MedicService, useValue: medicServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MedicUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize medicForm on ngOnInit', () => {
    expect(component.medicForm).toBeDefined();
  });

  it('should load medic data on ngOnInit', () => {
    const mockMedic: Medic = {
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1990-01-01',
      gender: 'Male',
      email: 'john@example.com',
      passwordHash: 'hash123',
      phoneNumber: '1234567890',
      address: '123 Main St',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'City Hospital'
    };
    medicServiceMock.getById.and.returnValue(of(mockMedic));
    
    component.ngOnInit();
    
    expect(medicServiceMock.getById).toHaveBeenCalledWith('1');
    expect(component.medicForm.value).toEqual({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1990-01-01',
      gender: 'Male',
      email: 'john@example.com',
      phoneNumber: '1234567890',
      address: '123 Main St',
      password: '',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'City Hospital'
    });
  });

  it('should handle error when loading medic data', () => {
    medicServiceMock.getById.and.returnValue(throwError('Error loading medic data'));
    
    spyOn(console, 'error');
    component.ngOnInit();

    expect(console.error).toHaveBeenCalledWith('Error loading medic data:', 'Error loading medic data');
    expect(routerMock.navigate).toHaveBeenCalledWith(['/medics']);
  });

  it('should submit the form and navigate on success', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1990-01-01',
      gender: 'Male',
      email: 'john@example.com',
      phoneNumber: '1234567890',
      address: '123 Main St',
      password: 'Password123!',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'City Hospital'
    });
    const token = 'mockToken';
    sessionStorage.setItem('jwtToken', token);

    medicServiceMock.update.and.returnValue(of({}));

    component.onSubmit();

    expect(medicServiceMock.update).toHaveBeenCalledWith('1', jasmine.any(Object), token);
    expect(routerMock.navigate).toHaveBeenCalledWith(['/medics']);
  });

  it('should handle error when submitting the form', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1990-01-01',
      gender: 'Male',
      email: 'john@example.com',
      phoneNumber: '1234567890',
      address: '123 Main St',
      password: 'Password123!',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'City Hospital'
    });
    const token = 'mockToken';
    sessionStorage.setItem('jwtToken', token);

    medicServiceMock.update.and.returnValue(throwError('Error updating medic'));

    spyOn(console, 'error');
    component.onSubmit();

    expect(console.error).toHaveBeenCalledWith('Error updating medic:', 'Error updating medic');
  });

  it('should handle no JWT token in session storage', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1990-01-01',
      gender: 'Male',
      email: 'john@example.com',
      phoneNumber: '1234567890',
      address: '123 Main St',
      password: 'Password123!',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'City Hospital'
    });

    sessionStorage.removeItem('jwtToken');

    spyOn(console, 'error');
    component.onSubmit();

    expect(console.error).toHaveBeenCalledWith('No JWT token found in session storage');
  });

  it('should display error messages for invalid form fields', () => {
    component.medicForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: 'invalid-email',
      phoneNumber: 'invalid-phone',
      address: '',
      password: 'short',
      rank: '',
      specialization: '',
      hospital: ''
    });
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('#firstName + .error')?.textContent).toContain('First Name is required');
    expect(compiled.querySelector('#lastName + .error')?.textContent).toContain('Last Name is required');
    expect(compiled.querySelector('#birthDate + .error')?.textContent).toContain('Birth Date is required');
    expect(compiled.querySelector('#gender + .error')?.textContent).toContain('Gender is required');
    expect(compiled.querySelector('#email + .error')?.textContent).toContain('Invalid email format');
    expect(compiled.querySelector('#phoneNumber + .error')?.textContent).toContain('Phone number is required');
    expect(compiled.querySelector('#address + .error')?.textContent).toContain('Address is required');
    expect(compiled.querySelector('#password + .error')?.textContent).toContain('Password must meet the required criteria');
    expect(compiled.querySelector('#rank + .error')?.textContent).toContain('Rank is required');
    expect(compiled.querySelector('#specialization + .error')?.textContent).toContain('Specialization is required');
    expect(compiled.querySelector('#hospital + .error')?.textContent).toContain('Hospital is required');
  });

  it('should disable submit button if form is invalid', () => {
    component.medicForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      password: '',
      rank: '',
      specialization: '',
      hospital: ''
    });
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const button = compiled.querySelector('button[type="submit"]') as HTMLButtonElement;
    expect(button.disabled).toBeTrue();
  });

  it('should enable submit button if form is valid', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1990-01-01',
      gender: 'Male',
      email: 'john@example.com',
      phoneNumber: '1234567890',
      address: '123 Main St',
      password: 'Password123!',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'City Hospital'
    });
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const button = compiled.querySelector('button[type="submit"]') as HTMLButtonElement;
    expect(button.disabled).toBeFalse();
  });

  it('should call loadMedicData if medicId is present', () => {
    spyOn(component, 'loadMedicData');
    component.ngOnInit();
    expect(component.loadMedicData).toHaveBeenCalled();
  });

  it('should not call loadMedicData if medicId is not present', () => {
    activatedRouteMock.snapshot.paramMap.get = () => null;
    spyOn(component, 'loadMedicData');
    component.ngOnInit();
    expect(component.loadMedicData).not.toHaveBeenCalled();
  });

  it('should call medicService.update with correct parameters', () => {
    component.medicForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '1990-01-01',
      gender: 'Male',
      email: 'john@example.com',
      phoneNumber: '1234567890',
      address: '123 Main St',
      password: 'Password123!',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'City Hospital'
    });
    const token = 'mockToken';
    sessionStorage.setItem('jwtToken', token);

    medicServiceMock.update.and.returnValue(of({}));

    component.onSubmit();

    expect(medicServiceMock.update).toHaveBeenCalledWith('1', jasmine.any(Object), token);
  });

  it('should not call medicService.update if form is invalid', () => {
    component.medicForm.setValue({
      firstName: '',
      lastName: '',
      birthDate: '',
      gender: '',
      email: '',
      phoneNumber: '',
      address: '',
      password: '',
      rank: '',
      specialization: '',
      hospital: ''
    });

    component.onSubmit();

    expect(medicServiceMock.update).not.toHaveBeenCalled();
  });
});