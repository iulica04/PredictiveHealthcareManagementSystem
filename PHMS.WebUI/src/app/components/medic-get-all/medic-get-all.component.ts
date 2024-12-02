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
  error: boolean = false;

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
    // Inițial nu încărcăm datele, așteptăm să fie trimis formularul
  }

  getMedics(): void {
    const { page, pageSize, rank, specialization } = this.medicForm.value;
    this.medicService.getAll(page, pageSize, rank, specialization).subscribe((data: { data: Medic[], totalCount: number }) => {
      console.log('Received data:', data); // Log the received data to the console
      this.medics = data.data;
      this.error = false;
      this.cdr.detectChanges(); // Force change detection
    }, error => {
      console.error('Error fetching medics:', error);
      this.medics = []; // Reset the medics array
      this.error = true;
      this.cdr.detectChanges(); // Force change detection
    });
  }

  onSubmit(): void {
    this.getMedics();
  }
}