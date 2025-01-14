import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ConsultationService } from '../../services/consultation.service';
import { Consultation } from '../../models/consultation.model';
import { MedicService } from '../../services/medic.service';
import { Medic } from '../../models/medic.model';
import { ConsultationStatus } from '../../models/consultation.model';
import { AbstractControl, ValidatorFn } from '@angular/forms'; // Add this line



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
  timeRangeValidator(min: string, max: string): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      if (!control.value) return null; // Nu valida dacă nu este completat
      
      const [hour, minute] = control.value.split(':').map(Number);
      const inputTime = hour * 60 + minute;
      
      const [minHour, minMinute] = min.split(':').map(Number);
      const minTime = minHour * 60 + minMinute;
      
      const [maxHour, maxMinute] = max.split(':').map(Number);
      const maxTime = maxHour * 60 + maxMinute;
      
      return inputTime >= minTime && inputTime <= maxTime ? null : { outOfRange: true };
    };
  }
  


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
      appointmentDate: [{ value: '', disabled: false }, Validators.required],
      appointmentTime: ['', [Validators.required, this.timeRangeValidator('08:00', '18:00')]],
      location: [{ value: '', disabled: !isMedic }, Validators.required], 
      status: [{ value: '', disabled: !isMedic }, Validators.required], 
      notes: [''] // Optional field
    });
  }
  
  loadUserRole(): void {
    // Assuming the role is stored in session storage as 'userRole'
    const role = sessionStorage.getItem('role');
  
    if (role === 'Medic') {
      this.consultationForm.get('appointmentDate')?.enable();
      this.consultationForm.get('appointmentTime')?.enable();
      this.consultationForm.get('status')?.enable();
      this.consultationForm.get('location')?.enable();
    } else if (role === 'Patient') {
      this.consultationForm.get('appointmentDate')?.enable();
      this.consultationForm.get('appointmentTime')?.enable();    }
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
              status:  0
            };
            const date = new Date(appointment.date); // Data primită de la server în UTC
            const appointmentTime = date.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' }); // Ora locală HH:MM
            const appointmentDate = date.toLocaleDateString('en-CA'); // Data locală YYYY-MM-DD

            this.consultationForm.patchValue({
              appointmentDate: appointmentDate,
              appointmentTime: appointmentTime,
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
        const date = this.consultationForm.get('appointmentDate')?.value;
  const time = this.consultationForm.get('appointmentTime')?.value;

  // Creează un obiect Date local din data și ora selectată
  const localDate = new Date(`${date}T${time}:00`);

  // Convertește data locală în UTC manual
  const utcDate = new Date(
    Date.UTC(
      localDate.getFullYear(),
      localDate.getMonth(),
      localDate.getDate(),
      localDate.getHours(),
      localDate.getMinutes()
    )
  );

  // Formatează data în format ISO (UTC)
  const fullAppointmentDate = utcDate.toISOString();

        let updateData: any = {
          id: this.appointmentId,
          date: fullAppointmentDate
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
  