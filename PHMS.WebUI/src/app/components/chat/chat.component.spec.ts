import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ChatComponent } from './chat.component';
import { ChatService } from '../../services/chat.service';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';

fdescribe('ChatComponent', () => {
  let component: ChatComponent;
  let fixture: ComponentFixture<ChatComponent>;
  let chatServiceMock: any;

  beforeEach(async () => {
    chatServiceMock = jasmine.createSpyObj('ChatService', ['getResponse']);
    chatServiceMock.getResponse.and.returnValue(of({ response: 'How can I assist you today?' }));

    await TestBed.configureTestingModule({
      imports: [FormsModule, CommonModule, ChatComponent],
      providers: [
        { provide: ChatService, useValue: chatServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ChatComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display initial assistant message', () => {
    const compiled = fixture.debugElement.nativeElement;
    const initialMessage = compiled.querySelector('.messages-container .message.assistant');
    expect(initialMessage).toBeTruthy();
    expect(initialMessage.textContent).toContain("👋 Hello! I'm your medical assistant. How can I help you today? 😊");
  });

  it('should clear userMessage after sending a message', () => {
    component.userMessage = 'Test message';
    component.sendMessage();

    expect(component.userMessage).toBe('');
  });

  it('should add assistant message after receiving response', () => {
    component.userMessage = 'Hello!';
    component.sendMessage();

    expect(component.messages.length).toBe(3);
    expect(component.messages[2]).toEqual({ sender: 'assistant', text: 'How can I assist you today?' });
  });

  it('should handle error from chat service', () => {
    chatServiceMock.getResponse.and.returnValue(throwError('API error'));
    component.userMessage = 'Hello!';
    component.sendMessage();

    expect(component.messages.length).toBe(3);
    expect(component.messages[2]).toEqual({ sender: 'assistant', text: 'A apărut o eroare. Te rog să încerci din nou.' });
  });

  it('should not send empty messages', () => {
    component.userMessage = '   '; // Message with only spaces
    component.sendMessage();

    expect(component.messages.length).toBe(1); // Only initial message should be present
  });

  it('should format messages correctly', () => {
    const formattedMessage = component.formatMessage('This is **bold** text');
    expect(formattedMessage).toBe('This is <b>bold</b> text');
  });

  it('should display all messages in the chat container', () => {
    component.userMessage = 'Hello!';
    component.sendMessage();
    fixture.detectChanges();

    const compiled = fixture.debugElement.nativeElement;
    const messages = compiled.querySelectorAll('.messages-container .message');
    expect(messages.length).toBe(3); // Initial message + user message + assistant response
  });
  
  it('should call sendMessage on enter key press', () => {
    spyOn(component, 'sendMessage').and.callThrough();
    const input = fixture.debugElement.query(By.css('input')).nativeElement;
    const event = new KeyboardEvent('keydown', { key: 'Enter' });
    input.dispatchEvent(event);
    expect(component.sendMessage).toHaveBeenCalled();
  });
});