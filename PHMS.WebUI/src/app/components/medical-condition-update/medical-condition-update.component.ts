import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MedicalConditionService } from '../../services/medical-condition.service';
import { CommonModule } from '@angular/common';
import { MedicalCondition } from '../../models/medicalCondition.model';

@Component({
  selector: 'app-medical-condition-update',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './medical-condition-update.component.html',
  styleUrls: ['./medical-condition-update.component.css']
})
export class MedicalConditionUpdateComponent implements OnInit {
  conditionForm: FormGroup;
  medicalConditionId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private medicalConditionService: MedicalConditionService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.conditionForm = this.fb.group({
      patientId: ['', [Validators.required]],
      name: ['', [Validators.required, Validators.maxLength(50)]],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      startDate: ['', [Validators.required]],
      endDate: [''],
      currentStatus: ['', [Validators.required]],
      isGenetic: [null, [Validators.required]],
      recommendation: ['', [Validators.required, Validators.maxLength(500)]],
      treatments: this.fb.array([]) // Initialize treatments as a FormArray
    });
  }

  ngOnInit(): void {
    this.medicalConditionId = this.route.snapshot.paramMap.get('id');
    if (this.medicalConditionId) {
      this.medicalConditionService.getMedicalConditionById(this.medicalConditionId).subscribe((data: MedicalCondition) => {
        this.conditionForm.patchValue(data);
        this.setTreatments(data.treatments);
      });
    }
  }

  setTreatments(treatments: any[]): void {
    const treatmentFormArray = this.conditionForm.get('treatments') as FormArray;
    treatments.forEach(treatment => {
      treatmentFormArray.push(this.fb.group({
        name: [treatment.name, Validators.required],
        type: [treatment.type, Validators.required],
        location: [treatment.location, Validators.required],
        startDate: [treatment.startDate, Validators.required],
        duration: [treatment.duration, Validators.required],
        frequency: [treatment.frequency, Validators.required],
        medications: this.fb.array(treatment.medications.map((medication: any) => this.createMedicationGroup(medication)))
      }));
    });
  }

  createMedicationGroup(medication: any): FormGroup {
    return this.fb.group({
      name: [medication.name, Validators.required],
      type: [medication.type, Validators.required],
      ingredients: [medication.ingredients, Validators.required],
      adverseEffects: [medication.adverseEffects, Validators.required]
    });
  }

  get treatments(): FormArray {
    return this.conditionForm.get('treatments') as FormArray;
  }

  getMedications(treatmentIndex: number): FormArray {
    return this.treatments.at(treatmentIndex).get('medications') as FormArray;
  }

  onSubmit(): void {
    if (this.conditionForm.valid) {
      const medicalCondition: MedicalCondition = {
        ...this.conditionForm.value,
        startDate: new Date(this.conditionForm.value.startDate).toISOString(),
        endDate: this.conditionForm.value.endDate ? new Date(this.conditionForm.value.endDate).toISOString() : undefined,
        isGenetic: this.conditionForm.value.isGenetic === 'true'
      };
      console.log('Updating medical condition:', medicalCondition);
      this.medicalConditionService.updateMedicalCondition(this.medicalConditionId, medicalCondition).subscribe({
        next: (response) => {
          console.log('Medical condition updated successfully:', response);
          this.router.navigate(['/']);
        },
        error: (error) => {
          console.error('Error updating medical condition:', error);
          if (error.error && error.error.errors) {
            console.error('Validation errors:', error.error.errors);
          }
        }
      });
    }
  }
}