import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { MedicListComponent } from './components/medic-list/medic-list.component';
import { MedicCreateComponent } from './components/medic-create/medic-create.component';
import { UserDetailComponent } from './components/user-detail/user-detail.component';
import { MedicUpdateComponent } from './components/medic-update/medic-update.component';
import { MedicGetAllComponent } from './components/medic-get-all/medic-get-all.component';
import { LoginComponent } from './components/login/login.component';
import { SpecializationsComponent } from './components/specializations/specializations.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';

export const appRoutes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'medics/register', component: MedicCreateComponent },
    { path: 'medics', component: MedicListComponent },
    { path: 'medics/create', component: MedicCreateComponent },
    { path: 'medics/paginated', component: MedicGetAllComponent },
    { path: 'users/:id', component: UserDetailComponent },
    { path: 'medics/update/:id', component: MedicUpdateComponent },
    { path: 'login', component: LoginComponent },
    { path: 'specialties', component: SpecializationsComponent },
    { path: 'forgot-password', component: ForgotPasswordComponent },
    { path: 'reset-password/:token', component: ResetPasswordComponent }
];