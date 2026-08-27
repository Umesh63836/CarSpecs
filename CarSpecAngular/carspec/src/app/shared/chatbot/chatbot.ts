import { Component, inject, signal } from '@angular/core';
import { IChatMessage } from '../../core/models/interfaces/ichat-message';
import { Chatservice } from '../../core/services/chatbot/chatservice';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-chatbot',
  imports: [FormsModule],
  templateUrl: './chatbot.html',
  styleUrl: './chatbot.css',
})
export class Chatbot {
  isOpen = signal(false);

  userMessage = '';

  messages = signal<IChatMessage[]>([
    {
      role: 'assistant',
      content: 'Hi! I am CarsSpec. AI Assistant. Ask me about cars, models, variants, prices or comparison.'
    }
  ]);

  isLoading = signal(false);

  chatservice = inject(Chatservice);

  switchChat() {
    this.isOpen.update(value => !value);
  }

  sendMessage() {
    const message = this.userMessage.trim();
    if (!message || this.isLoading()) {
      return;
    }
    this.messages.update(messages => [
      ...messages,
      { role: 'user',
        content: message }
    ]);
    this.userMessage = '';
    this.isLoading.set(true);
    this.chatservice.sendMessage(message).subscribe({next: response => {
        this.messages.update(messages => [
          ...messages,
          { role: 'assistant',
            content: response.message }
        ]);
        this.isLoading.set(false);
      },
      error: error => {this.messages.update(messages => [
          ...messages,
          { role: 'assistant',
            content: 'Sorry, something went wrong. Please try again.'}
        ]);
        this.isLoading.set(false);
      }
    });
  }

  onEnter() {
    this.sendMessage();
    this.userMessage = '';
  }

}
