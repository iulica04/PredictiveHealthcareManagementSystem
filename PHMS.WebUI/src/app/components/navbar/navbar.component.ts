import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common'; // Import CommonModule

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
  standalone: true,  // Since you're using standalone component
  imports: [CommonModule]  // Include CommonModule here
})
export class NavbarComponent implements OnInit {
  isLoggedIn: boolean = false;
  isPatient: boolean = false;

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.checkLoginStatus();
  }

  checkLoginStatus(): void {
    if (typeof sessionStorage !== 'undefined') {
      const token = sessionStorage.getItem('jwtToken');
      const userId = sessionStorage.getItem('userId');
      
      if (token && userId) {
        this.isLoggedIn = true;
        this.isPatient = sessionStorage.getItem('role') === 'Patient';  // Verifică rolul utilizatorului, presupunând că este salvat în sessionStorage
      }
    }
  }

  logout(): void {
    if (typeof sessionStorage !== 'undefined') {
      sessionStorage.removeItem('jwtToken');
      sessionStorage.removeItem('userId');
      sessionStorage.removeItem('role');
    }
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

  redirectToMyDetails(): void {
    if (typeof sessionStorage !== 'undefined') {
      const userId = sessionStorage.getItem('userId');  // Obține userId din sessionStorage
      if (userId) {
        this.router.navigate([`/patients/${userId}`]);
      }
    }
  }
}