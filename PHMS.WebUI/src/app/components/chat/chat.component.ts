import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from "../navbar/navbar.component";

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


  sendMessage() {
    if (this.userMessage.trim()) {
  
      this.messages.push({ sender: 'user', text: this.userMessage });

      this.userMessage = '';

      setTimeout(() => {
        this.messages.push({ sender: 'assistant', text: 'How can I assist you today?' });
      }, 1000);
    }
  }
}