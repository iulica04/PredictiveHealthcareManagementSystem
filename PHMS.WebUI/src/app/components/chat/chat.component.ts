import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { DiseasePredictionService } from '../../services/chat.service';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule, CommonModule, NavbarComponent],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent {
  userMessage: string = '';
  messages: { sender: string, text: string }[] = []; 

  constructor(private predictionService: DiseasePredictionService) { }

  sendMessage() {
    if (this.userMessage.trim()) {
      this.messages.push({ sender: 'user', text: this.userMessage });
      const symptoms = this.userMessage;

      this.predictionService.predictDisease(symptoms).subscribe(response => {
        console.log("Răspuns primit de la API:", response);
        this.messages.push({ sender: 'assistant', text: response.predictedDisease });
      }, error => {
        console.error("Eroare API:", error);
        this.messages.push({ sender: 'assistant', text: 'Error predicting disease. Please try again.' });
      });
      

      this.userMessage = '';
    }
  }
}
