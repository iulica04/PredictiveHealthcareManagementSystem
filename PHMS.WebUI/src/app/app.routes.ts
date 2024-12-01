import { Routes } from '@angular/router';
import { PatientListComponent } from './components/patient-list/patient-list.component';
import { PatientCreateComponent } from './components/patient-create/patient-create.component';
import { PatientUpdateComponent } from './components/patient-update/patient-update.component';
import { PatientDetailComponent } from './components/patient-detail/patient-detail.component';

export const appRoutes: Routes = [
    {path: '', redirectTo: '/patients', pathMatch: 'full'},
    {path: 'patients', component: PatientListComponent},
    {path: 'patients/create', component: PatientCreateComponent},
    { path: 'patients/update/:id', component: PatientUpdateComponent }, 
    { path: 'patients/:id', component: PatientDetailComponent }, 
  ];
