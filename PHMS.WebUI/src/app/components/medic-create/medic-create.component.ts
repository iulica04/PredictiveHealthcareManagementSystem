import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MedicService } from '../../services/medic.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-medic-create',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './medic-create.component.html',
  styleUrl: './medic-create.component.css'
})
export class MedicCreateComponent implements OnInit {
  medicForm: FormGroup;

  constructor(
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

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.medicForm.valid) {
      const medic = this.medicForm.value;
      console.log('Medic created:', medic);
      
      this.medicService.createMedic(this.medicForm.value).subscribe(() => {
        this.router.navigate(['/medics']);
      });
    }
  }
}
