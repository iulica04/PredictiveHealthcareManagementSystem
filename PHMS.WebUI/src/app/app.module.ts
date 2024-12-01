import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { appRoutes } from "./app.routes";
import { provideHttpClient } from "@angular/common/http";
import { PatientService} from "./services/patient.service";
import { RouterModule } from "@angular/router";

@NgModule({
    declarations: [
        
    ],
    imports: [
        BrowserModule,
        CommonModule,
        BrowserAnimationsModule,
        ReactiveFormsModule,
        RouterModule.forRoot(appRoutes),
    ],
    providers: [provideHttpClient(), PatientService],
    })
    export class AppModule { }