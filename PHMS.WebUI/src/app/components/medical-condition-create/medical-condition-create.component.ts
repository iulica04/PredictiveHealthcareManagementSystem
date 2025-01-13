import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MedicalConditionService } from '../../services/medical-condition.service';
import { CommonModule } from '@angular/common';
import { MedicalCondition } from '../../models/medicalCondition.model';

@Component({
  selector: 'app-medical-condition-create',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './medical-condition-create.component.html',
  styleUrls: ['./medical-condition-create.component.css']
})
export class MedicalConditionCreateComponent implements OnInit {
  conditionForm: FormGroup;
  patientId: string | null = null;

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
    this.patientId = this.route.snapshot.paramMap.get('id');
    if (this.patientId) {
      this.conditionForm.patchValue({ patientId: this.patientId });
    }
  }

  get treatments(): FormArray {
    return this.conditionForm.get('treatments') as FormArray;
  }

  addTreatment(): void {
    this.treatments.push(this.fb.group({
      name: ['', Validators.required],
      type: ['', Validators.required],
      location: ['', Validators.required],
      startDate: ['', Validators.required],
      duration: ['', Validators.required],
      frequency: ['', Validators.required]
    }));
  }

  removeTreatment(index: number): void {
    this.treatments.removeAt(index);
  }

  onSubmit(): void {
    if (this.conditionForm.valid) {
      const medicalCondition: MedicalCondition = {
        ...this.conditionForm.value,
        startDate: new Date(this.conditionForm.value.startDate).toISOString(),
        endDate: this.conditionForm.value.endDate ? new Date(this.conditionForm.value.endDate).toISOString() : undefined,
        isGenetic: this.conditionForm.value.isGenetic === 'true'
      };
      console.log('Creating medical condition:', medicalCondition);
      this.medicalConditionService.createMedicalCondition(medicalCondition).subscribe({
        next: (response) => {
          console.log('Medical condition created successfully:', response);
          this.router.navigate(['/']);
        },
        error: (error) => {
          console.error('Error creating medical condition:', error);
          if (error.error && error.error.errors) {
            console.error('Validation errors:', error.error.errors);
          }
        }
      });
    }
  }
}