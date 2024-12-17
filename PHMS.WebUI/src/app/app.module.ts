import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { appRoutes } from "./app.routes";
import { provideHttpClient } from "@angular/common/http";
import { PatientService} from "./services/patient.service";
import { RouterModule } from "@angular/router";
import { MedicService } from "./services/medic.service";
import { NavbarComponent } from "./components/navbar/navbar.component";

@NgModule({
    declarations: [
        NavbarComponent,
    ],
    imports: [
        BrowserModule,
        CommonModule,
        BrowserAnimationsModule,
        ReactiveFormsModule,
        RouterModule.forRoot(appRoutes),
    ],
    providers: [provideHttpClient(), PatientService, MedicService], 
    })
    export class AppModule { }