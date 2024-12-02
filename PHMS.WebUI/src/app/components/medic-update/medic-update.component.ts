import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MedicService } from '../../services/medic.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Medic } from '../../models/medic.model';

@Component({
  selector: 'app-medic-update',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './medic-update.component.html',
  styleUrl: './medic-update.component.css'
})
export class MedicUpdateComponent implements OnInit{
  medicForm: FormGroup;
  medicId!: string;

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private medicService: MedicService,
    private router: Router
  ) {
    this.medicForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(30)]],
      lastName: ['', [Validators.required, Validators.maxLength(30)]],
      birthDate: ['', Validators.required],
      gender: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\+?[0-9]{7,15}$/)]],
      address: ['', Validators.required],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.maxLength(100),
          Validators.pattern(/.*[A-Z].*/), 
          Validators.pattern(/.*[a-z].*/), 
          Validators.pattern(/.*[0-9].*/), 
          Validators.pattern(/.*[\W_].*/)  
        ]
      ],
      rank : ['', [Validators.required, Validators.maxLength(30)]],
      specialization : ['', [Validators.required, Validators.maxLength(30)]],
      hospital : ['', [Validators.required, Validators.maxLength(30)]]
    });
  }

  ngOnInit(): void {
    this.medicId = this.route.snapshot.paramMap.get('id') || '';
    if (this.medicId) {
      this.loadMedicData();
    }
  }

  loadMedicData(): void {
    this.medicService.getById(this.medicId).subscribe(
      (medic: Medic) => {
        this.medicForm.patchValue(medic);
      },
      (error) => {
        console.error('Error loading medic data:', error);
        this.router.navigate(['/medics']);
      }
    );
  }

  onSubmit(): void {
    if (this.medicForm.valid) {
      const UpdatesMedic : Medic = {...this.medicForm.value, id: this.medicId};
      this.medicService.update(this.medicId, UpdatesMedic).subscribe(() => {
        console.log('Medic updated successfully');
        this.router.navigate(['/medics']);
      },
      (error) => {
        console.error('Error updating medic:', error);
      });
    }
  }
}
