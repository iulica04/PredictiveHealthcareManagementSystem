import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MedicService } from '../../services/medic.service';
import { Medic } from '../../models/medic.model';

@Component({
  selector: 'app-medic-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './medic-detail.component.html',
  styleUrl: './medic-detail.component.css'
})
export class MedicDetailComponent implements OnInit{
  medic? : Medic;

  constructor(
    private route: ActivatedRoute,
    private medicService: MedicService,
    private router: Router
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.medicService.getById(id).subscribe((data) => {
        this.medic = data;
      });
    }
  }

  deleteMedic() {
    if (this.medic) {
      this.medicService.delete(this.medic.id).subscribe(() => {
        this.router.navigate(['/medics']);
      });
    }
  }
}
