import { Service, signal } from '@angular/core';
import { ApiError } from '../../models/interfaces/api-error';

@Service()
export class Error {
  error = signal<ApiError | null>(null);

  showError(status: number, message: string) {
    this.error.set({status,message});
    // Automatically remove after 5 seconds
    setTimeout(() => {
      this.clear();
    }, 5000);
  }

  clear() {
    this.error.set(null);
  }
}
