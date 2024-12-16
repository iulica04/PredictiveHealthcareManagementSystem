import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent {
  userMessage: string = ''; // Mesajul introdus de utilizator
  messages: { sender: string, text: string }[] = []; // Lista mesajelor

  // Funcție pentru a trimite un mesaj
  sendMessage() {
    if (this.userMessage.trim()) {
      // Adaugă mesajul utilizatorului în listă
      this.messages.push({ sender: 'user', text: this.userMessage });

      // Golește inputul
      this.userMessage = '';

      // Răspuns de test pentru asistent
      setTimeout(() => {
        this.messages.push({ sender: 'assistant', text: 'How can I assist you today?' });
      }, 1000);
    }
  }
}