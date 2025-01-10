import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user.model';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-user-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.css']
})
export class UserDetailComponent implements OnInit {
  user?: User;

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.userService.getById(id, sessionStorage.getItem('jwtToken')!).subscribe((data) => {
        this.user = data;
      }, error => {
        console.error('Error fetching user details:', error);
        this.user = undefined; // Reset user to undefined on error
      });
    }
  }
  
  deleteUser() {
    if (this.user?.id) {
      const token = sessionStorage.getItem('jwtToken'); // Retrieve the token from sessionStorage

      if (token) {
        this.userService.delete(this.user.id, token).subscribe(
          () => {
            console.log('User deleted successfully');
            this.router.navigate(['users']);
          },
          (error) => {
            console.error('Error deleting user:', error);
          }
        );
      } else {
        console.error('No JWT token found in session storage');
      }
    }
  }
  navigateToUpdateUser(id: string) {
    this.router.navigate([`users/update/${id}`]);
  }
  logout(): void {
    this.authService.logout();
  }
}