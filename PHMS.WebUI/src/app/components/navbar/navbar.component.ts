import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  constructor(private router: Router) {}

  redirectToLogin() {
    this.router.navigate(['/login']); // Navighează către ruta de login
  }

  redirectToGetMedics() {
    this.router.navigate(['/medics/paginated']); // Navighează către ruta de login
  }

  redirectToGetSpecializations(){
    this.router.navigate(['/specialties']);
  }
}
