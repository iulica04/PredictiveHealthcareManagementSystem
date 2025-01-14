import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MedicalConditionService } from '../../services/medical-condition.service';
import { MedicalCondition } from '../../models/medicalCondition.model';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';


@Component({
  selector: 'app-medical-condition-list',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl: './medical-condition-list.component.html',
  styleUrls: ['./medical-condition-list.component.css']
})
export class MedicalConditionListComponent implements OnInit {
  medicalConditions: MedicalCondition[] = [];
  patientId: string | null = null;

  constructor(
    private medicalConditionService: MedicalConditionService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.patientId = this.route.snapshot.paramMap.get('id');
    if (this.patientId) {
      this.medicalConditionService.getMedicalConditionsByPatientId(this.patientId).subscribe(
        (data: MedicalCondition[]) => {
          this.medicalConditions = data;
        },
        (error) => {
          console.error('Error fetching medical conditions:', error);
        }
      );
    }
  }

  navigateToUpdateMedicalCondition(medicalConditionId: string): void {
    this.router.navigate([`/medical-condition-update/${medicalConditionId}`]);
  }



}