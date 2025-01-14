import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MedicalConditionService } from '../../services/medical-condition.service';
import { MedicalCondition } from '../../models/medicalCondition.model';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';

enum TreatmentType {
  Surgery = 0,
  Therapy = 1,
  Medication = 2,
  Rehabilitation = 3,
  Other = 4
}

enum MedicationType {
  Tablet = 0,
  Capsule = 1,
  Liquid = 2,
  Injection = 3,
  Inhaler = 4,
  Topical = 5,
  Suppository = 6,
  Drops = 7,
  Other = 8
}

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
    @Inject(MedicalConditionService) private medicalConditionService: MedicalConditionService,
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

  getTreatmentType(type: TreatmentType): string {
    switch (type) {
      case TreatmentType.Surgery:
        return 'Surgery';
      case TreatmentType.Therapy:
        return 'Therapy';
      case TreatmentType.Medication:
        return 'Medication';
      case TreatmentType.Rehabilitation:
        return 'Rehabilitation';
      case TreatmentType.Other:
        return 'Other';
      default:
        return 'Unknown';
    }
  }

  getMedicationType(type: MedicationType): string {
    switch (type) {
      case MedicationType.Tablet:
        return 'Tablet';
      case MedicationType.Capsule:
        return 'Capsule';
      case MedicationType.Liquid:
        return 'Liquid';
      case MedicationType.Injection:
        return 'Injection';
      case MedicationType.Inhaler:
        return 'Inhaler';
      case MedicationType.Topical:
        return 'Topical';
      case MedicationType.Suppository:
        return 'Suppository';
      case MedicationType.Drops:
        return 'Drops';
      case MedicationType.Other:
        return 'Other';
      default:
        return 'Unknown';
    }
  }

  formatDuration(duration: string): string {
    const date = new Date(duration);
    return `${date.getDate()}/${date.getMonth() + 1}/${date.getFullYear()} ${date.getHours()}:${date.getMinutes()}`;
  }

  navigateToCreateMedicalCondition(): void {
    if (this.patientId) {
      this.router.navigate([`/medical-condition-create/${this.patientId}`]);
    }
  }

}