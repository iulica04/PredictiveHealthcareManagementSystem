import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ChatComponent } from './chat.component';

fdescribe('ChatComponent', () => {
  let component: ChatComponent;
  let fixture: ComponentFixture<ChatComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FormsModule, CommonModule, ChatComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(ChatComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should add user message to messages array on sendMessage', () => {
    component.userMessage = 'Hello, assistant!';
    component.sendMessage();

    expect(component.messages.length).toBe(1);
    expect(component.messages[0]).toEqual({ sender: 'user', text: 'Hello, assistant!' });
  });

  it('should clear userMessage after sending a message', () => {
    component.userMessage = 'Test message';
    component.sendMessage();

    expect(component.userMessage).toBe('');
  });

  it('should add assistant message after a delay', (done) => {
    component.userMessage = 'Hello!';
    component.sendMessage();

    setTimeout(() => {
      expect(component.messages.length).toBe(2);
      expect(component.messages[1]).toEqual({ sender: 'assistant', text: 'How can I assist you today?' });
      done();
    }, 1000);
  });

  it('should not send empty messages', () => {
    component.userMessage = '   '; // Message with only spaces
    component.sendMessage();

    expect(component.messages.length).toBe(0);
  });
});