import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { ChatService } from '../../services/chat.service';
import { FormsModule } from '@angular/forms';
import { Component, OnInit } from '@angular/core';


@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule, CommonModule, NavbarComponent],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})

export class ChatComponent implements OnInit {
  userMessage: string = '';
  messages: { sender: string, text: string }[] = []; 

  constructor(private chatService: ChatService) { }

  ngOnInit(): void {
    // Adaugă un mesaj inițial de introducere de la asistent
    this.messages.push({
      sender: 'assistant',
      text: "👋 Hello! I'm your medical assistant. How can I help you today? 😊"
    });
  }

  sendMessage() {
    if (this.userMessage.trim()) {
      const message = this.userMessage;

      // Adaugă mesajul utilizatorului în listă
      this.messages.push({ sender: 'user', text: message });

      // Trimite cererea către API și adaugă răspunsul asistentului
      this.chatService.getResponse(message).subscribe(
        (response) => {
          // Adaugă răspunsul asistentului în listă
          this.messages.push({ sender: 'assistant', text: response.response });
        },
        (error) => {
          console.error('Eroare la primirea răspunsului de la API:', error);
          this.messages.push({ sender: 'assistant', text: 'A apărut o eroare. Te rog să încerci din nou.' });
        }
      );

      // Resetează mesajul utilizatorului
      this.userMessage = '';
    }
  }
  formatMessage(text: string): string {
    return text.replace(/\*\*(.*?)\*\*/g, '<b>$1</b>');
  }
  
}
