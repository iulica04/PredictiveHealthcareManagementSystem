import { Component, OnInit, Input } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MedicalCondition } from '../../models/medicalCondition.model';
import { MedicalConditionService } from '../../services/medical-condition.service';
import { TreatmentType } from '../../models/treatment.model';
import { MedicationType } from '../../models/medication.model';

@Component({
  selector: 'app-medical-condition-get',
  templateUrl: './medical-condition-get.component.html',
  styleUrls: ['./medical-condition-get.component.css'],
  standalone: true,
  imports: [CommonModule, DatePipe]
})
export class MedicalConditionGetComponent implements OnInit {
  @Input() patientId: string | null = null;
  medicalConditions: MedicalCondition[] = [];

  constructor(private medicalConditionService: MedicalConditionService) {}

  ngOnInit(): void {
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
}