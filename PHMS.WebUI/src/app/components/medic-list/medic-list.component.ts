import { Component, OnInit } from '@angular/core';
import { Medic } from '../../models/medic.model';
import { MedicService } from '../../services/medic.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-medic-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './medic-list.component.html',
  styleUrl: './medic-list.component.css'
})
export class MedicListComponent implements OnInit {
  medics: Medic[] = [];
  constructor(private medicService: MedicService, private router: Router) { }

  ngOnInit(): void {
    this.medicService.getMedics().subscribe((data: Medic[]) => {
      this.medics = data;
    });
  }

  navigateToCreateMedic() {
    this.router.navigate(['medics/create']);
  }

  navigateToDetailMedic(id: string) {
    this.router.navigate([`medics/${id}`]);
  }

  navigateToUpdateMedic(id: string) {
    this.router.navigate([`medics/update/${id}`]);
  }

  navigateToPaginatedMedics() {
    this.router.navigate(['medics/paginated']);
  }

}