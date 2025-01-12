import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-role-selection',
  templateUrl: './role-selection.component.html',
  styleUrls: ['./role-selection.component.css']
})
export class RoleSelectionComponent {
  constructor(private router: Router) {}

  navigateToPatient(): void {
    this.router.navigate(['/patients/register']);
  }

  navigateToMedic(): void {
    this.router.navigate(['/medics/register']);
  }
}
