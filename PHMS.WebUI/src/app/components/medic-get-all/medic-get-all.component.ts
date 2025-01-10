import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { MedicService } from '../../services/medic.service';
import { Medic } from '../../models/medic.model';
import { NavbarComponent } from '../navbar/navbar.component';

@Component({
  selector: 'app-medic-get-all',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NavbarComponent],
  templateUrl: './medic-get-all.component.html',
  styleUrls: ['./medic-get-all.component.css']
})
export class MedicGetAllComponent implements OnInit {
  medics: Medic[] = [];
  filteredMedics: Medic[] = [];
  specializations: string[] = [
    'Cardiology', 'Neurology', 'Orthopedics', 'Pediatrics', 'Dermatology', 'Oncology', 'Gastroenterology', 
    'Urology', 'Psychiatry', 'Internal Medicine', 'Endocrinology', 'Hematology', 'Infectious Diseases', 
    'Geriatrics', 'Immunology', 'Radiology', 'Rheumatology', 'Rehabilitation Medicine', 'Sports Medicine', 
    'Intensive Care Medicine', 'Toxicology', 'Genetics', 'Nutrition', 'Allergy', 'Family Medicine', 
    'Plastic Surgery', 'General Surgery', 'Cardiovascular Surgery', 'Thoracic Surgery', 'Pediatric Surgery', 
    'Neurosurgery', 'Obstetrics and Gynecology', 'Maxillofacial Surgery', 'Vascular Surgery', 'Bariatric Surgery', 
    'Endoscopic Surgery', 'Orthopedic Surgery', 'Oncological Surgery', 'Spinal Surgery', 'Plastic and Reconstructive Surgery'
  ];
  selectedSpecialization: string = '';
  currentPage: number = 1;
  itemsPerPage: number = 12;
  totalCount: number = 0;

  constructor(
    private fb: FormBuilder,
    private medicService: MedicService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadMedics();
  }

  loadMedics(): void {
    this.medicService.getAll(this.currentPage, this.itemsPerPage, '', this.selectedSpecialization).subscribe((data: { data: Medic[], totalCount: number }) => {
      this.medics = data.data;
      this.filteredMedics = this.medics;
      this.totalCount = data.totalCount;
      this.cdr.detectChanges(); // Force change detection
    }, error => {
      console.error('Error fetching medics:', error);
      this.medics = []; // Reset the medics array
      this.filteredMedics = [];
      this.cdr.detectChanges(); // Force change detection
    });
  }

  filterBySpecialization(event: Event): void {
    const selectElement = event.target as HTMLSelectElement; 
    this.selectedSpecialization = selectElement.value; 
    this.currentPage = 1; // Reset to the first page
    this.loadMedics();
  }

  get paginatedMedics(): Medic[] {
    return this.filteredMedics;
  }

  nextPage(): void {
    if (this.currentPage * this.itemsPerPage < this.totalCount) {
      this.currentPage++;
      this.loadMedics();
      this.cdr.detectChanges(); // Force change detection
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadMedics();
      this.cdr.detectChanges();
    }
  }
}
