import { Component, OnInit } from '@angular/core';
import { Patient } from '../../models/patient.model';
import { ActivatedRoute, Router } from '@angular/router';
import { PatientService } from '../../services/patient.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-patient-detail',
  standalone: true,
  imports: [ CommonModule ],
  templateUrl: './patient-detail.component.html',
  styleUrl: './patient-detail.component.css'
})
export class PatientDetailComponent implements OnInit {
  patient?: Patient;

  constructor(
    private route: ActivatedRoute,
    private patientService: PatientService,
    private router: Router
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      const token = sessionStorage.getItem('jwtToken'); // Retrieve the token from sessionStorage

      if (token) {
        this.patientService.getById(id, token).subscribe((data) => {
          this.patient = data;
        });
      } else {
        console.error('No JWT token found in session storage');
      }
    }
  }

  deletePatient() {
    if (this.patient) {
      this.patientService.delete(this.patient.id).subscribe(() => {
        this.router.navigate(['/patients']);
      });
    }
  }
  navigateToUpdatePatient(id: string) {
    this.router.navigate([`patients/update/${id}`]);
  }
  logout(): void {
    this.patientService.logout();
  }
}