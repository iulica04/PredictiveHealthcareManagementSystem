import { Component, OnInit } from '@angular/core';
import { Patient } from '../../models/patient.model';
import { PatientService } from '../../services/patient.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-patient-get-all',
  standalone: true,
  imports: [],
  templateUrl: './patient-get-all.component.html',
  styleUrl: './patient-get-all.component.css'
})
export class PatientGetAllComponent implements OnInit {
  
  patients: Patient[] = [];
  page = 1;
  pageSize = 10;
  totalItems = 0;

  constructor(private patientService: PatientService , private router: Router) {}

  ngOnInit() {
    this.loadPatients();
  }

  loadPatients() {
    this.patientService.getAll(this.page, this.pageSize).subscribe((data) => {
      this.patients = data.items;
      this.totalItems = data.totalCount;
    });
  }

  onPageChange(page: number) {
    this.page = page;
    this.loadPatients();
  }
}