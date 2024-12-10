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
  styleUrls: ['./medic-detail.component.css']
})
export class MedicDetailComponent implements OnInit {
  medic?: Medic;

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
      }, error => {
        console.error('Error fetching medic details:', error);
        this.medic = undefined; // Reset medic to undefined on error
      });
    }
  }
  
  deleteMedic() {
    if (this.medic?.id) {
      const token = sessionStorage.getItem('jwtToken'); // Retrieve the token from sessionStorage

      if (token) {
        this.medicService.delete(this.medic.id, token).subscribe(
          () => {
            console.log('Medic deleted successfully');
            this.router.navigate(['medics']);
          },
          (error) => {
            console.error('Error deleting medic:', error);
          }
        );
      } else {
        console.error('No JWT token found in session storage');
      }
    }
  }
  navigateToUpdateMedic(id: string) {
    this.router.navigate([`medics/update/${id}`]);
  }

}