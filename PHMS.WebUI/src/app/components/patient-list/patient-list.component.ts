import { CommonModule} from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Patient } from '../../models/patient.model';
import { PatientService } from '../../services/patient.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-patient-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './patient-list.component.html',
  styleUrl: './patient-list.component.css'
})
export class PatientListComponent implements OnInit {

  patients: Patient[] = [];
  constructor(private patientService: PatientService, private router : Router) {}
  
  ngOnInit(): void {
    this.patientService.getPatients().subscribe((data : Patient[]) => {
      this.patients = data
    });
  }
    
  navigateToCreatePatient() {
    this.router.navigate(['patients/create']);
  }

  navigateToDetailPatient(id: string) {
    this.router.navigate([`patients/${id}`]);
  }

  navigateToUpdatePatient(id: string) {
    this.router.navigate([`patients/update/${id}`]);
  }

}
