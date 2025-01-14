import { Component, OnInit, HostListener } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
  standalone: true,
  imports: [CommonModule]
})
export class NavbarComponent implements OnInit {
  isLoggedIn: boolean = false;
  isPatient: boolean = false;
  isMedic: boolean = false;
  menuOpen: boolean = false;
  isSmallScreen: boolean = false;

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.checkLoginStatus();
    this.checkScreenSize();
  }

  checkLoginStatus(): void {
    const token = sessionStorage.getItem('jwtToken');
    const userId = sessionStorage.getItem('userId');
    const role = sessionStorage.getItem('role');
    if (token && userId) {
      this.isLoggedIn = true;
      this.isPatient = role === 'Patient';
      this.isMedic = role === 'Medic';
    }
  }

  @HostListener('window:resize', [])
  checkScreenSize(): void {
    this.isSmallScreen = window.innerWidth <= 768; // Ecran mic sub 768px
    if (!this.isSmallScreen) {
      this.menuOpen = false; // Ascunde meniul pe ecrane mari
    }
  }

  toggleMenu(): void {
    this.menuOpen = !this.menuOpen;
  }

  logout(): void {
    sessionStorage.removeItem('jwtToken');
    sessionStorage.removeItem('userId');
    sessionStorage.removeItem('role');
    this.isLoggedIn = false;
    this.router.navigate(['/']);
  }

  redirectToGetChat(): void {
    this.router.navigate(['/chat']);
  }

  redirectToGetMedics(): void {
    this.router.navigate(['/medics/paginated']);
  }

  redirectToGetSpecializations(): void {
    this.router.navigate(['/specialties']);
  }

  redirectToGetConsultations(): void {
    this.router.navigate(['/consultations']);
  }

  redirectToLogin(): void {
    this.router.navigate(['/login']);
  }

  redirectToPatientList(): void {
    this.router.navigate(['/patients']);
  }

  redirectToMyDetails(): void {
    const userId = sessionStorage.getItem('userId');
    const role = sessionStorage.getItem('role');
    if (role === 'Patient') {
      this.router.navigate([`/patients/${userId}`]);
    } else if (role === 'Medic') {
      this.router.navigate([`/medics/${userId}`]);
    }
  }
}