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
      this.patientService.getById(id).subscribe((data) => {
        this.patient = data;
      });
    }
  }

  deletePatient() {
    if (this.patient) {
      this.patientService.delete(this.patient.id).subscribe(() => {
        this.router.navigate(['/patients']);
      });
    }
  }
}