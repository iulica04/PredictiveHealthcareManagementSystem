import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { UserType } from '../../models/userType.model';
import { User } from '../../models/user.model';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-medic-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './medic-list.component.html',
  styleUrl: './medic-list.component.css'
})
export class MedicListComponent implements OnInit {
  medics: User[] = [];
  constructor(private userService: UserService, private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.userService.getUsersByType(UserType.Medic).subscribe((data: User[]) => {
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
  logout(): void {
    this.authService.logout();
  }

}