import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ConsultationService } from '../../services/consultation.service';
import { Consultation } from '../../models/consultation.model';
import { MedicService } from '../../services/medic.service';
import { Medic } from '../../models/medic.model';
import { ConsultationStatus } from '../../models/consultation.model';


@Component({
  selector: 'app-update-consultation',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './update-consultation.component.html',
  styleUrls: ['./update-consultation.component.css']
})
export class UpdateConsultationComponent implements OnInit {
  consultationForm!: FormGroup;
  currentStep: number = 1;
  nextClicked: boolean = false;
  initialAppointmentData: any = {}; // Obiect pentru a stoca valorile inițiale
  currentImage: string = 'assets/images/image2.png';
  medics: Medic[] = [];  
  patient?: any;  // Define your patient model type
  isLoading: boolean = false;
  appointmentId: string | null = null;
  role: any = '';
  ConsultationStatus = ConsultationStatus; 
  statusOptions = [
    { value: ConsultationStatus.Pending, label: 'Pending' },
    { value: ConsultationStatus.Accepted, label: 'Accepted' },
    { value: ConsultationStatus.Cancelled, label: 'Cancelled' },
    { value: ConsultationStatus.Done, label: 'Done' },
    { value: ConsultationStatus.Declined, label: 'Declined' }
  ];


  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private consultationService: ConsultationService,
    private medicService: MedicService,
    private router: Router,
    
  ) {}

  ngOnInit(): void {
    this.role = sessionStorage.getItem('role'); // Assume 'medic' or 'patient' is stored
    this.appointmentId = this.route.snapshot.paramMap.get('id');
    this.initializeForm();
    this.loadAppointmentDetails();
  }
  
  initializeForm(): void {
    const isMedic = this.role === 'Medic';

    this.consultationForm = this.formBuilder.group({
      date: [{ value: '', disabled: false }, Validators.required], 
      location: [{ value: '', disabled: !isMedic }, Validators.required], 
      status: [{ value: '', disabled: !isMedic }, Validators.required], 
      notes: [''] // Optional field
    });
  }
  loadUserRole(): void {
    // Assuming the role is stored in session storage as 'userRole'
    const role = sessionStorage.getItem('role');
  
    if (role === 'Medic') {
      this.consultationForm.get('date')?.enable();
      this.consultationForm.get('status')?.enable();
      this.consultationForm.get('location')?.enable();
    } else if (role === 'Patient') {
      this.consultationForm.get('date')?.enable();
    }
  }
 
  
    loadAppointmentDetails(): void {
      this.isLoading = true;
      const token = sessionStorage.getItem('jwtToken');
    
      if (token && this.appointmentId) {
        this.consultationService.getAppointmentById(this.appointmentId).subscribe(
          (appointment: Consultation) => {
            this.initialAppointmentData = { 
            id: this.appointmentId,
              location: appointment.location, 
              status: appointment.status 
            };
            
            this.consultationForm.patchValue({
              date: appointment.date,
              location: appointment.location,
              status: appointment.status
            });
    
            console.log('Appointment details loaded:', appointment);
            this.isLoading = false;
          },
          (error) => {
            console.error('Error loading appointment details:', error);
            this.isLoading = false;
          }
        );
      }
    }
  
    updateAppointment(): void {
      if (this.consultationForm.invalid) {
        return;
      }
    
      const token = sessionStorage.getItem('jwtToken');
      const userRole = sessionStorage.getItem('role');
    
      if (token && this.appointmentId) {
        const appointmentDate = this.consultationForm.get('date')?.value;
        const formattedDate = new Date(appointmentDate).toISOString();
    
        let updateData: any = {
          id: this.appointmentId,
          date: formattedDate
        };
    
        // Dacă utilizatorul este medic, include câmpurile modificate
        if (userRole === 'Medic') {
          updateData = {
            ...updateData,
            location: this.consultationForm.get('location')?.value,
            status: Number(this.consultationForm.get('status')?.value),
          };
        }
    
        // Dacă utilizatorul este pacient, adaugă valorile inițiale pentru location și status
        if (userRole === 'Patient') {
          updateData = {
            ...updateData,
            location: this.initialAppointmentData.location,
            status: this.initialAppointmentData.status
          };
        }
    
        console.log('Updating appointment with data:', updateData); // Verifică datele înainte de a le trimite
    
        this.consultationService.updateConsultation(this.appointmentId, updateData).subscribe(
          () => {
            console.log('Appointment updated successfully');
            this.router.navigate(['']); // Navighează la pagina de detalii
          },
          (error) => {
            console.error('Error updating appointment:', error);
          }
        );
      }
    }
  
    onSubmit(): void {
      if (this.consultationForm.valid) {
        this.updateAppointment();
      } else {
        console.log('Form is invalid');
      }
    }
  }
  