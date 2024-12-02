import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MedicService } from '../../services/medic.service';
import { Medic } from '../../models/medic.model';

@Component({
  selector: 'app-medic-get-all',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './medic-get-all.component.html',
  styleUrls: ['./medic-get-all.component.css']
})
export class MedicGetAllComponent implements OnInit {
  medics: Medic[] = [];
  medicForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private medicService: MedicService,
    private cdr: ChangeDetectorRef
  ) {
    this.medicForm = this.fb.group({
      page: [1],
      pageSize: [10],
      rank: [''],
      specialization: ['']
    });
  }

  ngOnInit(): void {
    const { page, pageSize, rank, specialization } = this.medicForm.value;
    this.medicService.getAll(page, pageSize, rank, specialization).subscribe((data: { data: Medic[], totalCount: number } ) => {
      this.medics = data.data;
      this.cdr.detectChanges(); 
    }, error => {
      console.error('Error fetching medics:', error);
    });
  }

  onSubmit(): void {
    this.ngOnInit();
  }
}