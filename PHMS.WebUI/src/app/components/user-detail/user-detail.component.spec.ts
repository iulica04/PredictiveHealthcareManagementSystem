import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UserService } from '../../services/user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { CommonModule } from '@angular/common';
import { UserDetailComponent } from './user-detail.component';
import { UserType } from '../../models/userType.model';

fdescribe('UserDetailComponent', () => {
  let component: UserDetailComponent;
  let fixture: ComponentFixture<UserDetailComponent>;
  let userServiceMock: any;
  let routerMock: any;
  let activatedRouteMock: any;

  beforeEach(async () => {
    // Mock UserService
    userServiceMock = jasmine.createSpyObj('UserService', ['getById', 'delete']);
    userServiceMock.getById.and.returnValue(of({
      id: '1',
      type: UserType.Medic,
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'General Hospital',
      passwordHash: 'hashedpassword'
    }));

    // Mock Router
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    // Mock ActivatedRoute
    activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: () => '1' // Mock the ID parameter
        }
      }
    };

    await TestBed.configureTestingModule({
      imports: [CommonModule, UserDetailComponent],
      providers: [
        { provide: UserService, useValue: userServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UserDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load user details on init', () => {
    expect(userServiceMock.getById).toHaveBeenCalledWith('1');
    expect(component.user).toEqual({
      id: '1',
      type: UserType.Medic,
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'General Hospital',
      passwordHash: 'hashedpassword'
    });
  });

  it('should display loading message when user details are not yet loaded', () => {
    component.user = undefined;
    fixture.detectChanges();
    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('p').textContent).toContain('Loading user details...');
  });

  it('should call deleteUser and navigate to users list on delete', () => {
    userServiceMock.delete.and.returnValue(of({}));
    component.deleteUser();
    expect(userServiceMock.delete).toHaveBeenCalledWith('1');
    expect(routerMock.navigate).toHaveBeenCalledWith(['users']);
  });

  it('should handle error when fetching user details', () => {
    userServiceMock.getById.and.returnValue(throwError('Error fetching user details'));
    component.ngOnInit();
    fixture.detectChanges();
    expect(component.user).toBeUndefined();
    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('p').textContent).toContain('Loading user details...');
  });

  it('should handle error when deleting user', () => {
    userServiceMock.delete.and.returnValue(throwError('Error deleting user'));
    component.user = {
      id: '1',
      type: UserType.Medic,
      firstName: 'John',
      lastName: 'Doe',
      birthDate: '2000-01-01',
      gender: 'Male',
      email: 'john.doe@example.com',
      phoneNumber: '+1234567890',
      address: '123 Main St',
      rank: 'Senior',
      specialization: 'Cardiology',
      hospital: 'General Hospital',
      passwordHash: 'hashedpassword'
    };
    component.deleteUser();
    fixture.detectChanges();
    expect(userServiceMock.delete).toHaveBeenCalledWith('1');
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('should not call deleteUser if user is undefined', () => {
    component.user = undefined;
    component.deleteUser();
    expect(userServiceMock.delete).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });
});